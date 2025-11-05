using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;

public sealed record OrderFulfilled : IDomainEvent
{
    public OrderFulfilled(Guid id, decimal tradePrice, int tradeQuantity, DateTime utcNow)
    {
        Id = id;
        TradePrice = tradePrice;
        TradeQuantity = tradeQuantity;
        FulfilledAt = utcNow;
    }

    public OrderFulfilled() { }

    public Guid Id { get; init; }
    public decimal TradePrice { get; init; }
    public int TradeQuantity { get; init; }
    public DateTime FulfilledAt { get; init; }
}
