using PopcornMarket.OrderBook.Domain.Helpers;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.OrderBook.Domain.Entities;

/// <summary>
/// The OrderBook is responsible for managing buy and sell
/// orders for a specific stock. 
/// </summary>
public class OrderBook : Entity
{
    public string StockSymbol { get; private set; } = null!;
    private readonly SortedSet<BuyOrder> _buyOrders = new();
    private readonly SortedSet<SellOrder> _sellOrders = new();

    public IReadOnlyCollection<BuyOrder> BuyOrders => _buyOrders;
    public IReadOnlyCollection<SellOrder> SellOrders => _sellOrders;
    
    private OrderBook(string stockSymbol)
    {
        StockSymbol = stockSymbol;
        _buyOrders = new SortedSet<BuyOrder>(new OrderComparer(true));
        _sellOrders = new SortedSet<SellOrder>(new OrderComparer(false));
    }

    public static OrderBook Create(string stockSymbol)
    {
        if(string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentNullException(nameof(stockSymbol));

        return new OrderBook(stockSymbol);
    }
    
    public BuyOrder AddBuyOrder(string traderId, decimal price, int quantity)
    {
        var buyOrder = BuyOrder.Create(StockSymbol, traderId, price, quantity, this);
        _buyOrders.Add(buyOrder);
        return buyOrder;
    }

    public SellOrder AddSellOrder(string traderId, decimal price, int quantity)
    {
        var sellOrder = SellOrder.Create(StockSymbol, traderId, price, quantity, this);
        _sellOrders.Add(sellOrder);
        return sellOrder;
    }

    public Order? GetBestBuyOrder() => _buyOrders.LastOrDefault();
    public Order? GetBestSellOrder() => _sellOrders.FirstOrDefault();
}
