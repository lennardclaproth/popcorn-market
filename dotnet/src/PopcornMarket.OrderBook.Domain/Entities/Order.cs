using PopcornMarket.OrderBook.Domain.Enums;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.OrderBook.Domain.Entities;

/// <summary>
/// Represents a buy or sell request placed by a trader.
/// </summary>
public class Order : Entity
{
    public Guid OrderBookId { get; private set; }
    public OrderBook OrderBook { get; private set; } = null!;
    public string StockSymbol { get; private set; } = null!;
    public string TraderId { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public DateTime Timestamp { get; private set; }
    public OrderStatus Status { get; private set; }
    
    protected Order() { } // Required for EF Core

    protected Order(string stockSymbol, string traderId, decimal price, int quantity, OrderBook orderBook) : base(Guid.NewGuid())
    {
        OrderBook = orderBook;
        OrderBookId = orderBook.Id;
        StockSymbol = stockSymbol;
        TraderId = traderId;
        Price = price;
        Quantity = quantity;
        Timestamp = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }
    
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 0) throw new ArgumentException("Quantity cannot be negative.");
        Quantity = newQuantity;
    }
    
    public void FulfillOrder()
    {
        Status = OrderStatus.Fulfilled;
    }

    public void PartiallyFulfillOrder(int newQuantity)
    {
        if (newQuantity <= 0) FulfillOrder();
        else
        {
            Quantity = newQuantity;
            Status = OrderStatus.PartiallyFilled;
        }
    }
}
