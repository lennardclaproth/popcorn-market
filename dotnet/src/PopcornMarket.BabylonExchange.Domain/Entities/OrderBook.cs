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

    public void MatchOrder(Order order)
    {
        // Decide which book to match against
        var oppositeOrders = order.OrderSide == OrderSide.Buy 
            ? _sellOrders 
            : _buyOrders;

        bool wasPartiallyFilled = false;

        while (order.RemainingQuantity > 0 && oppositeOrders.Count != 0)
        {
            var bestMatch = order.OrderSide == OrderSide.Buy
                ? GetBestSellOrder()
                : GetBestBuyOrder();

            if (bestMatch == null)
                break;

            // Price check (skip if limit price not satisfied)
            if (!IsPriceMatch(order, bestMatch))
            {
                // Limit order cannot match, so it rests
                if (order.OrderType == OrderType.LimitOrder)
                {
                    RestOrder(order);
                }
                break;
            }

            // Execute trade
            var tradeQuantity = Math.Min(order.RemainingQuantity, bestMatch.RemainingQuantity);

            // Default to the price of the limit order (the liquidity provider)
            decimal tradePrice;

            if (order.OrderType == OrderType.MarketOrder && bestMatch.OrderType == OrderType.LimitOrder)
            {
                tradePrice = bestMatch.Price;
            }
            else if (order.OrderType == OrderType.LimitOrder && bestMatch.OrderType == OrderType.MarketOrder)
            {
                tradePrice = order.Price;
            }
            else if (order.OrderType == OrderType.LimitOrder && bestMatch.OrderType == OrderType.LimitOrder)
            {
                // For two limit orders, trade at the resting (bestMatch) price
                tradePrice = bestMatch.Price;
            }
            else
            {
                // Both are market orders → no valid trade
                throw new InvalidOperationException("Cannot execute a trade between two market orders.");
            }

            order.TryFulfillOrder(tradePrice, tradeQuantity);
            bestMatch.TryFulfillOrder(tradePrice, tradeQuantity);

            // Track partial fill
            if (tradeQuantity > 0) wasPartiallyFilled = true;

            // Determine what events to publish
            if (order.Status == OrderStatus.Fulfilled)
                RaiseDomainEvent(new OrderFulfilled(order.Id, tradePrice, tradeQuantity, DateTime.UtcNow));
            else
                RaiseDomainEvent(new OrderPartiallyFilled(order.Id, order.RemainingQuantity, tradePrice, DateTime.UtcNow));

            if (bestMatch.Status == OrderStatus.Fulfilled)
                RaiseDomainEvent(new OrderFulfilled(bestMatch.Id, tradePrice, tradeQuantity, DateTime.UtcNow));
            else
                RaiseDomainEvent(new OrderPartiallyFilled(bestMatch.Id, bestMatch.RemainingQuantity, tradePrice, DateTime.UtcNow));

            // Correctly assign BuyOrderId and SellOrderId
            if (order.OrderSide == OrderSide.Buy)
                RaiseDomainEvent(new TradeExecuted()
                {
                    BuyOrderId = order.Id,
                    SellOrderId = bestMatch.Id,
                    TradePrice = tradePrice,
                    TradeQuantity = tradeQuantity,
                    StockSymbol = StockSymbol,
                    ExecutedAt = DateTime.UtcNow
                });
            else
            {
                RaiseDomainEvent(new TradeExecuted()
                {
                    BuyOrderId = bestMatch.Id,
                    SellOrderId = order.Id,
                    TradePrice = tradePrice,
                    TradeQuantity = tradeQuantity,
                    StockSymbol = StockSymbol,
                    ExecutedAt = DateTime.UtcNow
                });
            }
            if (bestMatch.Status == OrderStatus.Fulfilled ||
                bestMatch.Status == OrderStatus.Canceled ||
                bestMatch.Status == OrderStatus.PartiallyCanceled)
            {
                oppositeOrders.Remove(bestMatch);
                _orders.Remove(bestMatch);
            }

            if (order.Status == OrderStatus.Fulfilled ||
                order.Status == OrderStatus.Canceled ||
                order.Status == OrderStatus.PartiallyCanceled)
            {
                // If order itself is done, remove it as well
                if (order.OrderSide == OrderSide.Buy)
                    _buyOrders.Remove(order);
                else
                    _sellOrders.Remove(order);

                _orders.Remove(order);
            }
        }

        // Handle leftovers
        if (order.RemainingQuantity > 0)
        {
            if (order.OrderType == OrderType.LimitOrder)
            {
                // Limit orders can still rest
                RestOrder(order);
            }
            else
            {
                // Market order leftover = cancellation
                if (wasPartiallyFilled)
                {
                    RaiseDomainEvent(new OrderPartiallyCancelled
                    {
                        OrderId = order.Id,
                        Reason = $"Not able to match orders completely, order partially fulfilled. Remaining quantity: {order.RemainingQuantity}",
                        CancelledAt = DateTime.UtcNow,
                        RemainingQuantity = order.RemainingQuantity
                    });
                    order.PartiallyCancelOrder($"Not able to match orders completely, order partially fulfilled. Remaining quantity: {order.RemainingQuantity}", order.RemainingQuantity, DateTime.UtcNow);
                }
                else
                {
                    RaiseDomainEvent(new OrderCancelled(order.Id, "Not able to match orders, no matching orders.", DateTime.UtcNow));
                    order.CancelOrder("Not able to match orders, no matching orders.", DateTime.UtcNow);
                }
            }
        }
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

    private static bool IsPriceMatch(Order incoming, Order resting)
    {
        if (incoming.OrderSide == OrderSide.Buy)
        {
            if (incoming.OrderType == OrderType.MarketOrder) return true;

            if (incoming.Price >= resting.Price) return true;

            return false;
        }

        if (incoming.OrderSide == OrderSide.Sell)
        {
            if (incoming.OrderType == OrderType.MarketOrder || resting.OrderType == OrderType.MarketOrder) return true;

            if (incoming.Price <= resting.Price) return true;

            return false;
        }

        return false;
    }

    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault(o => o.OrderType == OrderType.LimitOrder);
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault(o => o.OrderType == OrderType.LimitOrder);
}
