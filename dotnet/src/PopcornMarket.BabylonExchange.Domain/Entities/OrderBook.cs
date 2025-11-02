using System.ComponentModel.DataAnnotations.Schema;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
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

    public Result MatchOrder(Order incomingOrder)
    {
        // Decide which book to match against, if it is a buy incomingOrder we look at the 
        // sell side if it is a sell incomingOrder we look at the buy side.
        var oppositeOrders = incomingOrder.OrderSide == OrderSide.Buy 
            ? _sellOrders 
            : _buyOrders;

        bool wasPartiallyFilled = false;

        // while the incomingOrder still has a remaining quantity and there are still opposing
        // orders left we keep trying to execute incomingOrder. By getting the bestMatch,
        while (incomingOrder.RemainingQuantity > 0)
        {
            if (oppositeOrders.Count == 0)
            {
                return Result.Failure(OrderBookErrors.OrderBookMatchOrderCacheMiss);
            }

            // Get the best match based on the incomingOrder side.
            var bestMatch = incomingOrder.OrderSide == OrderSide.Buy
                ? GetBestSellOrder()
                : GetBestBuyOrder();

            if (bestMatch == null)
                break;

            // If there is no price match we rest the incomingOrder on the orderbook.
            // only limit orders can rest, market orders get cancelled if they cannot be matched.
            if (!IsPriceMatch(incomingOrder, bestMatch))
            {
                // Limit incomingOrder cannot match, so it rests
                if (incomingOrder.OrderType == OrderType.LimitOrder)
                {
                    RestOrder(incomingOrder);
                }
                break;
            }

            // Calculate the quantity of the trader, how many stocks are gonna be traded.
            var tradeQuantity = Math.Min(incomingOrder.RemainingQuantity, bestMatch.RemainingQuantity);

            // Get the trade price
            var tradePrice = GetTradePrice(incomingOrder, bestMatch);

            // Fulfill both the incoming order and bestmatch.
            incomingOrder.TryFulfillOrder(tradePrice, tradeQuantity);
            bestMatch.TryFulfillOrder(tradePrice, tradeQuantity);

            // If the trade quantity is bigger than 0 we know that there was at least a partial fill.
            // therefore we track if it was partially filled.
            if (tradeQuantity > 0) wasPartiallyFilled = true;

            // Determine what events to publish
            RaiseOrderMatchedEvents(incomingOrder, bestMatch, tradePrice, tradeQuantity);
            CleanUpMatchedOrders(incomingOrder, bestMatch);
        }

        // If remaining quantity is 0 than the order has completely been fulfilled.
        // We can exit here.
        if (incomingOrder.RemainingQuantity == 0)
        {
            return Result.Success();
        }

        // If we reach here it means that there are no more matching orders but there is still parts of the
        // trade remaining. If it is a limit order we can rest it on the orderbook.
        if (incomingOrder.OrderType == OrderType.LimitOrder)
        {
            RestOrder(incomingOrder);
            return Result.Success();
        }

        // If it is a market order we have to cancel the remaining quantity.
        if (wasPartiallyFilled)
        {
            RaiseDomainEvent(new OrderPartiallyCancelled
            {
                OrderId = incomingOrder.Id,
                Reason = $"Not able to match orders completely, incomingOrder partially fulfilled. Remaining quantity: {incomingOrder.RemainingQuantity}",
                CancelledAt = DateTime.UtcNow,
                RemainingQuantity = incomingOrder.RemainingQuantity
            });
            incomingOrder.PartiallyCancelOrder($"Not able to match orders completely, incomingOrder partially fulfilled. Remaining quantity: {incomingOrder.RemainingQuantity}", incomingOrder.RemainingQuantity, DateTime.UtcNow);
            return Result.Success();
        }

        // No fills at all, cancel the order
        RaiseDomainEvent(new OrderCancelled(incomingOrder.Id, "Not able to match orders, no matching orders.", DateTime.UtcNow));
        incomingOrder.CancelOrder("Not able to match orders, no matching orders.", DateTime.UtcNow);
        return Result.Success();
    }

    /// <summary>
    /// Rests an order on the orderbook, makes sure that it is added to the correct side. Also prevents
    /// a domain event from being raised here, as this is done when the order is initially placed.
    /// </summary>
    /// <param name="order"></param>
    public void RestOrder(Order order)
    {
        if (order.OrderSide == OrderSide.Buy)
        {
            _buyOrders.Add(order);
        }
        else
        {
            _sellOrders.Add(order);
        }
    }

    public void EvictOrders(IEnumerable<Order> orders)
    {
        foreach (var order in orders)
        {
            _orders.Remove(order);
            if (order.OrderSide == OrderSide.Buy)
                _buyOrders.Remove(order);
            else
                _sellOrders.Remove(order);
        }
    }

    public IReadOnlyList<Order> OrdersOutsideWindow(int window)
    {
        var coldOrders = new List<Order>();

        if (_buyOrders.Count > window)
        {
            coldOrders.AddRange(_buyOrders.Skip(window));
        }

        if (_sellOrders.Count > window)
        {
            coldOrders.AddRange(_sellOrders.Skip(window));
        }

        return coldOrders;
    }


    /// <summary>
    /// We match the price. If the incoming order is a market order we can always return true.
    /// If the incoming order is a buy order we check if the price is greater than or equal to the resting order.
    /// If the incoming order is a sell order we return the price is less than or equal to the resting order or
    /// market order.
    /// </summary>
    /// <param name="incoming"></param>
    /// <param name="resting"></param>
    /// <returns></returns>
    private static bool IsPriceMatch(Order incoming, Order resting)
    {
        if (incoming.OrderType == OrderType.MarketOrder)
            return true;

        if (incoming.OrderSide == OrderSide.Buy)
            return incoming.Price >= resting.Price;

        if (incoming.OrderSide == OrderSide.Sell)
            return resting.OrderType == OrderType.MarketOrder || incoming.Price <= resting.Price;

        return false;
    }

    /// <summary>
    /// We determine the trade price based on the incomingOrder types. If one is a market incomingOrder we 
    /// use the price of the limit incomingOrder. If both are limit orders we use the price of the resting incomingOrder
    /// this is because the resting incomingOrder is deemed to be the best price. When two market incomingOrder
    /// are being traded we throw an exception, this should not be possible.
    /// </summary>
    /// <param name="incomingOrder">the incoming order to be matched</param>
    /// <param name="bestMatch">the best match for the order</param>
    /// <returns>the best price for the trade</returns>
    /// <exception cref="InvalidOperationException">the exception thrown when two market orders are traded.</exception>
    private static decimal GetTradePrice(Order incomingOrder, Order bestMatch)
    {
        decimal tradePrice;
        if (incomingOrder.OrderType == OrderType.MarketOrder && bestMatch.OrderType == OrderType.LimitOrder)
        {
            tradePrice = bestMatch.Price;
        }
        else if (incomingOrder.OrderType == OrderType.LimitOrder && bestMatch.OrderType == OrderType.MarketOrder)
        {
            tradePrice = incomingOrder.Price;
        }
        else if (incomingOrder.OrderType == OrderType.LimitOrder && bestMatch.OrderType == OrderType.LimitOrder)
        {
            tradePrice = bestMatch.Price;
        }
        else
        {
            throw new InvalidOperationException("Cannot execute a trade between two market orders.");
        }

        return tradePrice;
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

    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault(o => o.OrderType == OrderType.LimitOrder);
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
}
