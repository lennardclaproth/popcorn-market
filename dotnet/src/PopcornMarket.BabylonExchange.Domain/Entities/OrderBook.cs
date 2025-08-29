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
    
    private OrderBook(string ticker)
    {
        Ticker = ticker;
        _buyOrders = new SortedSet<Order>(new OrderComparer(true));
        _sellOrders = new SortedSet<Order>(new OrderComparer(false));
    }

    public static Result<OrderBook> Create(string ticker)
    {
        if(string.IsNullOrWhiteSpace(ticker)) throw new ArgumentNullException(nameof(ticker));

        return Result<OrderBook>.Success(new OrderBook(ticker));
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

#pragma warning disable CA1822
#pragma warning disable IDE0060
    public void MatchOrder(Order order)
#pragma warning restore IDE0060
#pragma warning restore CA1822
    {
        return;
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
