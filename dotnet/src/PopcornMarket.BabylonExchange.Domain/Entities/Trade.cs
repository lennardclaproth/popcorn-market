using System;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.BabylonExchange.Domain.Entities;

public sealed class Trade : Entity
{
    public Guid BuyOrderId { get; private set; }
    public Guid SellOrderId { get; private set; }
    public string StockSymbol { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public DateTime ExecutedAt { get; private set; }
    public Order? BuyOrder { get; private set; }
    public Order? SellOrder { get; private set; }

    private Trade() { } // Required by EF

    private Trade(Guid buyOrderId, Guid sellOrderId, string stockSymbol, decimal price, int quantity) 
        : base(Guid.NewGuid())
    {
        // Change to result pattern
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        // Change to result pattern
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");

        BuyOrderId = buyOrderId;
        SellOrderId = sellOrderId;
        StockSymbol = stockSymbol;
        Price = price;
        Quantity = quantity;
        ExecutedAt = DateTime.UtcNow;
    }

    public static Trade Create(Guid buyOrderId, Guid sellOrderId, string stockSymbol, decimal price, int quantity)
        => new Trade(buyOrderId, sellOrderId, stockSymbol, price, quantity);
}

