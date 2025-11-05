using System.ComponentModel.DataAnnotations.Schema;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.BabylonExchange.Domain.Helpers;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

/// <summary>
/// The OrderBook is responsible for managing buy and sell
/// orders for a specific stock. 
/// </summary>
public sealed class OrderBook : AggregateRoot
{
    public string StockSymbol { get; private set; } = null!;
    private readonly SortedSet<Order> _buyOrders = new(new OrderComparer(true));
    private readonly SortedSet<Order> _sellOrders = new(new OrderComparer(false));
    private readonly List<Order> _orders = new();
    public Listing Listing { get; private set; } = null!;
    public Guid ListingId { get; private set; }
    [NotMapped]
    public IReadOnlyCollection<Order> BuyOrders => _buyOrders;
    [NotMapped]
    public IReadOnlyCollection<Order> SellOrders => _sellOrders;
    public IReadOnlyCollection<Order> Orders => _orders;
    private OrderBook() { }
    private OrderBook(string stockSymbol, Listing listing)
    {
        StockSymbol = stockSymbol;
        ListingId = listing.Id;
        

        _buyOrders = new SortedSet<Order>(new OrderComparer(true));
        _sellOrders = new SortedSet<Order>(new OrderComparer(false));
    }

    public static Result<OrderBook> Create(Listing listing, string stockSymbol)
    {
        if(string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentNullException(nameof(stockSymbol));
        
        return Result<OrderBook>.Success(new OrderBook(stockSymbol, listing));
    }

    public void PlaceOrder(Order order)
    {
        _orders.Add(order);

        if (order.OrderSide == OrderSide.Buy)
        {
            _buyOrders.Add(order);
        } else
        {
            _sellOrders.Add(order);
        }

        var orderPlacedEvent = new OrderPlaced
        {
            OrderId = order.Id,
            StockSymbol = order.StockSymbol,
            Price = order.Price,
            Quantity = order.Quantity,
            Side = order.OrderSide,
            Type = order.OrderType,
        };
        RaiseDomainEvent(orderPlacedEvent);
    }

    /// <summary>
    /// When we match an order there are several different cases. We have a market order, a 
    /// </summary>
    /// <param name="incomingOrder"></param>
    /// <returns></returns>
    public bool MatchOrder(Order incomingOrder)
    {
        var bestMatch = GetBestMatch(incomingOrder);
        if (bestMatch == null)
        {
            return false;
        }

        // Calculate the quantity of the trader, how many stocks are going to be traded.
        var tradeQuantity = Math.Min(incomingOrder.RemainingQuantity, bestMatch.RemainingQuantity);

        var tradePrice = GetTradePrice(incomingOrder, bestMatch);

        incomingOrder.TryFulfillOrder(tradePrice, tradeQuantity);
        bestMatch.TryFulfillOrder(tradePrice, tradeQuantity);
        RaiseOrderMatchedEvents(incomingOrder, bestMatch, tradePrice, tradeQuantity);
        CleanUpMatchedOrders(incomingOrder, bestMatch);
        return true;
    }

    /// <summary>
    /// Cancels an order based on the status of the order. If it is partially filled it will publish
    /// an OrderPartiallyCancelledEvent and vice versa.
    /// </summary>
    /// <param name="incomingOrder"></param>
    public void CancelOrder(Order incomingOrder)
    {
        if (incomingOrder.Status == OrderStatus.PartiallyFilled)
        {
            EvictOrder(incomingOrder);
            RaiseDomainEvent(new OrderPartiallyCancelled
            {
                OrderId = incomingOrder.Id,
                Reason = $"Not able to match orders completely, incomingOrder partially fulfilled. Remaining quantity: {incomingOrder.RemainingQuantity}",
                CancelledAt = DateTime.UtcNow,
                RemainingQuantity = incomingOrder.RemainingQuantity
            });
            incomingOrder.PartiallyCancelOrder($"Not able to match orders completely, incomingOrder partially fulfilled. Remaining quantity: {incomingOrder.RemainingQuantity}", incomingOrder.RemainingQuantity, DateTime.UtcNow);
            return;
        }

        if (incomingOrder.OrderType == OrderType.MarketOrder)
        {
            EvictOrder(incomingOrder);
            RaiseDomainEvent(new OrderCancelled(incomingOrder.Id, "Not able to match orders, no matching orders.", DateTime.UtcNow));
            incomingOrder.CancelOrder("Not able to match orders, no matching orders.", DateTime.UtcNow);
        }
    }

    /// <summary>
    /// Rests an order on the orderbook, makes sure that it is added to the correct side. Also prevents
    /// a domain event from being raised here, as this is done when the order is initially placed.
    /// </summary>
    /// <param name="order"></param>
    public void RestOrder(Order order)
    {
        if (order.OrderType == OrderType.MarketOrder)
        {
            throw new InvalidOperationException("Cannot rest market order on the orderbook.");
        }
        var targetSet = order.OrderSide == OrderSide.Buy ? _buyOrders : _sellOrders;
        targetSet.RemoveWhere(o => o.Id == order.Id);
        targetSet.Add(order);
    }

    public void EvictOrder(Order order)
    {
        _orders.RemoveAll(o => o.Id == order.Id);
        if (order.OrderSide == OrderSide.Buy)
        {
            _buyOrders.Remove(order);
        }
        else
        {
            _sellOrders.Remove(order);
        }
    }

    /// <summary>
    /// We determine the trade price based on the incomingOrder types. If one is a market incomingOrder we 
    /// use the price of the limit incomingOrder. If both are limit orders we use the best price, this is
    /// dependent on the order type and the best match. When two market incomingOrder are being traded we
    /// throw an exception, this should not be possible.
    /// </summary>
    /// <param name="incomingOrder">the incoming order to be matched</param>
    /// <param name="bestMatch">the best match for the order</param>
    /// <returns>the best price for the trade</returns>
    /// <exception cref="InvalidOperationException">the exception thrown when two market orders are traded.</exception>
    private static decimal GetTradePrice(Order incomingOrder, Order bestMatch)
    {
        switch (incomingOrder.OrderType)
        {
            case OrderType.MarketOrder:
                if (bestMatch.OrderType == OrderType.LimitOrder) return bestMatch.Price;
                break;
            case OrderType.LimitOrder:
                if (bestMatch.OrderType == OrderType.MarketOrder) return incomingOrder.Price;
                if (incomingOrder.OrderSide == OrderSide.Sell) return Math.Max(incomingOrder.Price, bestMatch.Price);
                if (incomingOrder.OrderSide == OrderSide.Buy) return Math.Min(incomingOrder.Price, bestMatch.Price);
                break;
        }

        throw new InvalidOperationException("Cannot execute a trade between two market orders.");
    }

    private void RaiseOrderMatchedEvents(Order incomingOrder, Order bestMatch, decimal tradePrice, int tradeQuantity)
    {
        void RaiseFillEvent(Order order)
        {
            var now = DateTime.UtcNow;

            if (order.Status == OrderStatus.Fulfilled)
            {
                var orderFilledEvent = new OrderFulfilled(order.Id, tradePrice, tradeQuantity, now)
                {
                    FulfilledAt = now, Id = order.Id, TradePrice = tradePrice, TradeQuantity = tradeQuantity
                };
                RaiseDomainEvent(orderFilledEvent);
            }
            else
            {
                var orderPartiallyFilledEvent = new OrderPartiallyFilled()
                {
                    Id = order.Id, RemainingQuantity = order.RemainingQuantity, TradePrice = tradePrice, FulfilledAt = now
                };
                RaiseDomainEvent(orderPartiallyFilledEvent);
            }
        }

        // Fill events
        RaiseFillEvent(incomingOrder);
        RaiseFillEvent(bestMatch);

        // Trade event
        var tradeEvent = new TradeExecuted
        {
            BuyOrderId = incomingOrder.OrderSide == OrderSide.Buy ? incomingOrder.Id : bestMatch.Id,
            SellOrderId = incomingOrder.OrderSide == OrderSide.Buy ? bestMatch.Id : incomingOrder.Id,
            TradePrice = tradePrice,
            TradeQuantity = tradeQuantity,
            StockSymbol = StockSymbol,
            ExecutedAt = DateTime.UtcNow
        };

        RaiseDomainEvent(tradeEvent);
    }

    private void CleanUpMatchedOrders(Order incomingOrder, Order bestMatch)
    {
        bool ShouldRemove(Order order)
        {
            if (order.Status is OrderStatus.Fulfilled or OrderStatus.Canceled or OrderStatus.PartiallyCanceled)
            {
                return true;
            }

            return false;
        }

        switch (incomingOrder.OrderSide)
        {
            case OrderSide.Buy:
                if (ShouldRemove(incomingOrder)) _buyOrders.Remove(incomingOrder);
                if (ShouldRemove(bestMatch)) _sellOrders.Remove(bestMatch);
                break;
            case OrderSide.Sell:
                if (ShouldRemove(incomingOrder)) _sellOrders.Remove(incomingOrder);
                if (ShouldRemove(bestMatch)) _buyOrders.Remove(bestMatch);
                break;
        }
    }

    /// <summary>
    /// Gets the best match for the incoming order. When an order is of type marketOrder we return the highest price for incoming sell orders
    /// and the lowest price for incoming buy orders. When the order type is market order we return the same as the market order but where
    /// the price is at least bigger or smaller than the incoming order.
    /// </summary>
    /// <param name="incomingOrder">the order to be matched against</param>
    /// <returns>the best match or null when no match is found</returns>
    private Order? GetBestMatch(Order incomingOrder)
    {
        switch (incomingOrder.OrderType)
        {
            case OrderType.MarketOrder:
                if (incomingOrder.OrderSide == OrderSide.Sell) return _buyOrders.FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
                if (incomingOrder.OrderSide == OrderSide.Buy) return _sellOrders.FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
                break;
            case OrderType.LimitOrder:
                if (incomingOrder.OrderSide == OrderSide.Sell) return _buyOrders.Where(o => incomingOrder.Price <= o.Price).FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
                if (incomingOrder.OrderSide == OrderSide.Buy) return _sellOrders.Where(o => incomingOrder.Price >= o.Price).FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
                break;
        }

        return null;
    }
}
