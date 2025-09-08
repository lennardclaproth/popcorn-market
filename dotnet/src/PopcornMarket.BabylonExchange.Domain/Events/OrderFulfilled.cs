using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;

public record OrderFulfilled : IDomainEvent
{
    public OrderFulfilled(Guid id, decimal tradePrice, int tradeQuantity, DateTime fulfilledAt)
    {
        Id = id;
        TradePrice = tradePrice;
        TradeQuantity = tradeQuantity;
        FulfilledAt = fulfilledAt;
    }

    public Guid Id { get; }
    public decimal TradePrice { get; }
    public int TradeQuantity { get; }
    public DateTime FulfilledAt { get; }
}
