using PopcornMarket.OrderBook.Domain.Enums;

namespace PopcornMarket.OrderBook.Domain.Entities;

public class BuyOrder : Order
{
    private BuyOrder(){}
    
    private BuyOrder(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderBook orderBook) : base(stockSymbol, traderId, price, quantity, orderBook)
    {
        
    }

    public static BuyOrder Create(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderBook orderBook)
    {
        if (string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentException("Stock symbol is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");

        return new BuyOrder(stockSymbol, traderId, price, quantity, orderBook);
    }
}
