using System;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;

public record OrderPlaced : IDomainEvent
{
    public Guid OrderId { get; init; }
    public string StockSymbol { get; init; } = null!;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public OrderSide Side { get; init; }
    public OrderType Type { get; init; }
}
