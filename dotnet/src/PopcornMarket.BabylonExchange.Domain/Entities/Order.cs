using System.Globalization;
using PopcornMarket.BabylonExchange.Domain.Constants;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

/// <summary>
/// Represents a buy or sell request placed by a trader.
/// </summary>
public class Order : Entity
{
    public string OrderId { get; private set; } = null!;
    public Guid OrderBookId { get; private set; }
    public OrderBook OrderBook { get; private set; } = null!;
    public string StockSymbol { get; private set; } = null!;
    public string TraderId { get; private set; } = null!;
    public decimal Price { get; private set; }  
    public decimal ExecutionPrice { get; private set; }
    public int Quantity { get; private set; }
    public int RemainingQuantity { get; private set; }
    public DateTime PlacedTimestamp { get; private set; }
    public DateTime? ExecutedTimestamp { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? StatusNote { get; private set; }
    public OrderType OrderType { get; private set; }
    public OrderSide OrderSide { get; private set; }

    private Order() { } // Required for EF Core

    protected Order(string stockSymbol,
        string traderId,
        decimal price,
        int quantity,
        OrderType orderType,
        OrderBook orderBook,
        OrderSide orderSide) : base(Guid.NewGuid())
    {
        var now = DateTime.UtcNow;
        var millis = now.Millisecond.ToString("D3", new CultureInfo("en-US"));
        var random = new Random().Next(0, 999).ToString("D3", new CultureInfo("en-US"));

        OrderId = $"ORD-{now:yyyyMMddHHmmss}{millis}-{random}-{ExchangeConstants.ExchangeIdentifier}";
        OrderBook = orderBook;
        OrderBookId = orderBook.Id;
        StockSymbol = stockSymbol;
        TraderId = traderId;
        Price = price;
        Quantity = quantity;
        RemainingQuantity = quantity;
        PlacedTimestamp = DateTime.UtcNow;
        OrderType = orderType;
        Status = OrderStatus.Pending;
        OrderSide = orderSide;
    }

    public static Order Create(
        string stockSymbol,
        string traderId,
        decimal price,
        int quantity,
        OrderType orderType,
        OrderBook orderBook,
        OrderSide orderSide)
    {
        // Validation of input parameters
        if (string.IsNullOrWhiteSpace(stockSymbol)) throw new ArgumentException("Stock symbol is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (price <= 0 && orderType == OrderType.LimitOrder) throw new ArgumentException("Price must be greater than zero on a limit order.");
        if (price != 0 && orderType == OrderType.MarketOrder) throw new ArgumentException("Price must be zero on a market order.");

        return new Order( stockSymbol, traderId, price, quantity, orderType , orderBook, orderSide);
    }
    
    public void TryFulfillOrder(decimal price, int tradeQuantity)
    {
        var remainingQuantity = RemainingQuantity - tradeQuantity;
        if(remainingQuantity < 0)
            throw new InvalidOperationException("Trade quantity exceeds remaining order quantity.");

        if (remainingQuantity > 0)
        {
            RemainingQuantity = remainingQuantity;
            Status = OrderStatus.PartiallyFilled;
            return;
        }

        ExecutedTimestamp = DateTime.UtcNow;
        ExecutionPrice = price;
        RemainingQuantity = 0;
        Status = OrderStatus.Fulfilled;
    }

    public void FulfillOrder(decimal price, DateTime fulfilledAt)
    {
        if (Status == OrderStatus.Fulfilled)
            throw new InvalidOperationException("Order is already fulfilled.");
        RemainingQuantity = 0;
        ExecutionPrice = price;
        Status = OrderStatus.Fulfilled;
        ExecutedTimestamp = fulfilledAt;
    }

    public void PartiallyFulfillOrder(decimal price, int tradeQuantity, DateTime fulfilledAt)
    {
        if (Status == OrderStatus.Fulfilled)
            throw new InvalidOperationException("Order is already fulfilled.");
        
        RemainingQuantity -= tradeQuantity;
        ExecutionPrice = price;
        Status = OrderStatus.PartiallyFilled;
        ExecutedTimestamp = fulfilledAt;
    }

    public void CancelOrder(string reason, DateTime cancelledAt)
    {
        if (Status == OrderStatus.Fulfilled)
            throw new InvalidOperationException("Cannot cancel a fulfilled order.");
        Status = OrderStatus.Canceled;
        StatusNote = reason;
        ExecutedTimestamp = cancelledAt;
    }

    public void PartiallyCancelOrder(string reason, int newQuantity, DateTime cancelledAt)
    {
        if (Status == OrderStatus.Fulfilled)
            throw new InvalidOperationException("Cannot cancel a fulfilled order.");
        if (newQuantity <= 0 || newQuantity > RemainingQuantity)
            throw new ArgumentException("New quantity must be greater than zero and less than the remaining quantity.");
        
        RemainingQuantity = newQuantity;
        Status = OrderStatus.PartiallyCanceled;
        StatusNote = reason;
        ExecutedTimestamp = cancelledAt;
    }
}
