namespace PopcornMarket.OrderBook.Domain.Entities;

public class SellOrder : Order
{
    private SellOrder(){}
    
    private SellOrder(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderBook orderBook) : base(stockSymbol, traderId, price, quantity, orderBook)
    {
        
    }

    public static SellOrder Create(
        string stockSymbol, 
        string traderId, 
        decimal price, 
        int quantity, 
        OrderBook orderBook)
    {
        if (string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentException("Stock symbol is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");

        return new SellOrder(stockSymbol, traderId, price, quantity, orderBook);
    }
}
