using PopcornMarket.OrderBook.Domain.Enums;
using PopcornMarket.OrderBook.Domain.Helpers;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.OrderBook.Domain.Entities;

/// <summary>
/// The OrderBook is responsible for managing buy and sell
/// orders for a specific stock. 
/// </summary>
public sealed class OrderBook : Entity
{
    public string Ticker { get; private set; } = null!;
    private readonly SortedSet<BuyOrder> _buyOrders = new();
    private readonly SortedSet<SellOrder> _sellOrders = new();

    public IReadOnlyCollection<BuyOrder> BuyOrders => _buyOrders;
    public IReadOnlyCollection<SellOrder> SellOrders => _sellOrders;
    
    private OrderBook(string ticker)
    {
        Ticker = ticker;
        _buyOrders = new SortedSet<BuyOrder>(new OrderComparer(true));
        _sellOrders = new SortedSet<SellOrder>(new OrderComparer(false));
    }

    public static Result<OrderBook> Create(string ticker)
    {
        if(string.IsNullOrWhiteSpace(ticker)) throw new ArgumentNullException(nameof(ticker));

        return Result<OrderBook>.Success(new OrderBook(ticker));
    }
    
    public BuyOrder AddBuyOrder(string traderId, decimal price, int quantity, OrderType orderType)
    {
        var buyOrder = BuyOrder.Create(Ticker, traderId, price, quantity, orderType, this);
        _buyOrders.Add(buyOrder);
        return buyOrder;
    }

    public SellOrder AddSellOrder(string traderId, decimal price, int quantity, OrderType orderType)
    {
        var sellOrder = SellOrder.Create(Ticker, traderId, price, quantity, orderType, this);
        _sellOrders.Add(sellOrder);
        return sellOrder;
    }

    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault();
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault();
}
