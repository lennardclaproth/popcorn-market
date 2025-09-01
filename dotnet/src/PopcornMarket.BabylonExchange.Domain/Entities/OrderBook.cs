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
    public string Ticker { get; private set; } = null!;
    private readonly SortedSet<Order> _buyOrders = new();
    private readonly SortedSet<Order> _sellOrders = new();
    private readonly List<Order> _orders = new();
    public Listing Listing { get; private set; } = null!;
    public Guid ListingId { get; private set; }
    public decimal? CurrentPrice { get; private set; }

    [NotMapped]
    public IReadOnlyCollection<Order> BuyOrders => _buyOrders;
    [NotMapped]
    public IReadOnlyCollection<Order> SellOrders => _sellOrders;
    public IReadOnlyCollection<Order> Orders => _orders;
    private OrderBook() { }
    private OrderBook(string ticker, Listing listing)
    {
        Ticker = ticker;
        ListingId = listing.Id;
        
        _buyOrders = new SortedSet<Order>(new OrderComparer(true));
        _sellOrders = new SortedSet<Order>(new OrderComparer(false));
    }

    public static Result<OrderBook> Create(Listing listing, string ticker)
    {
        if(string.IsNullOrWhiteSpace(ticker)) throw new ArgumentNullException(nameof(ticker));
        
        return Result<OrderBook>.Success(new OrderBook(ticker, listing));
    }

    public void AddOrder(Order order)
    {
        _orders.Add(order);

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

        while (order.RemainingQuantity > 0 && oppositeOrders.Count != 0)
        {
            var bestMatch = order.OrderSide == OrderSide.Buy
                ? GetBestSellOrder()
                : GetBestBuyOrder();

            if (bestMatch == null)
                // Should we rest the order here?
                break;

            // Price check (skip if limit price not satisfied)
            if (!IsPriceMatch(order, bestMatch))
            {
                // Limit order cannot match, so it rests
                if (order.OrderType == OrderType.LimitOrder)
                {
                    // Should use the RestOrder function
                    AddOrder(order);
                }
                break;
            }

            // Execute trade
            var tradeQuantity = Math.Min(order.RemainingQuantity, bestMatch.RemainingQuantity);
            var tradePrice = bestMatch.Price; // Matching engine rule: execution at resting order price

            order.PartiallyFulfillOrder(order.RemainingQuantity - tradeQuantity, tradePrice);
            bestMatch.PartiallyFulfillOrder(bestMatch.RemainingQuantity - tradeQuantity, tradePrice);
            
            // Determine what events to publish
            if (order.Status == OrderStatus.Fulfilled)
                RaiseDomainEvent(new OrderFulfilled(order.Id, tradePrice, tradeQuantity));
            else
                RaiseDomainEvent(new OrderPartiallyFilled(order.Id, order.RemainingQuantity));

            if (bestMatch.Status == OrderStatus.Fulfilled)
                RaiseDomainEvent(new OrderFulfilled(bestMatch.Id, tradePrice, tradeQuantity));
            else
                RaiseDomainEvent(new OrderPartiallyFilled(bestMatch.Id, bestMatch.RemainingQuantity));

            RaiseDomainEvent(new TradeExecuted(order.Id, bestMatch.Id, tradePrice, tradeQuantity));

            if (bestMatch.Status == OrderStatus.Fulfilled)
                oppositeOrders.Remove(bestMatch);
        }

        // If incoming limit order still has remaining quantity → rest in the book
        if (order is { RemainingQuantity: > 0, OrderType: OrderType.LimitOrder })
        {
            // Should become rest function
            AddOrder(order);
        }
    }

    private static bool IsPriceMatch(Order incoming, Order resting)
    {
        return incoming.OrderSide switch
        {
            OrderSide.Buy => incoming.OrderType == OrderType.MarketOrder || incoming.Price >= resting.Price,
            OrderSide.Sell => incoming.OrderType == OrderType.MarketOrder || incoming.Price <= resting.Price,
            _ => false
        };
    }
    
    public Result SetReferencePrice(decimal referencePrice)
    {
        if (CurrentPrice != null)
        {
            return Result.Failure(OrderBookErrors.OrderBookReferencePriceSet);
        }
        CurrentPrice = referencePrice;
        return Result.Success();
    }
    
    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault();
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault();
}
