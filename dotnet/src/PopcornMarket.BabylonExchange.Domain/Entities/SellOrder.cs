using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

public sealed class SellOrder : Order
{
    public OrderType OrderType { get; private set; }
    
    private SellOrder(){}
    
    private SellOrder(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderType orderType,
        OrderBook orderBook) : base(stockSymbol, traderId, price, quantity, orderBook)
    {
        OrderType = orderType;
    }

    public static SellOrder Create(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderType orderType,
        OrderBook orderBook)
    {
        if (string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentException("Stock symbol is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");

        return new SellOrder(stockSymbol, traderId, price, quantity, orderType ,orderBook);
    }
}
