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
    public string Ticker { get; private set; } = null!;
    private readonly SortedSet<Order> _buyOrders = new();
    private readonly SortedSet<Order> _sellOrders = new();
    public decimal? CurrentPrice { get; private set; }

    public IReadOnlyCollection<Order> BuyOrders => _buyOrders;
    public IReadOnlyCollection<Order> SellOrders => _sellOrders;
    
    public Listing Listing { get; private set; } = null!;
    public Guid ListingId { get; private set; }
    
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
        switch (order.OrderSide)
        {
            case OrderSide.Buy:
                _buyOrders.Add(order);
                break;
            case OrderSide.Sell:
                _sellOrders.Add(order);
                break;
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
        throw new NotImplementedException();
    }
    
    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault();
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault();
}
