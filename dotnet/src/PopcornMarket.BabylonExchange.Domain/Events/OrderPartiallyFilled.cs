using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed class OrderPartiallyFilled : IDomainEvent
{
    public OrderPartiallyFilled(Guid id, int remainingQuantity, decimal tradePrice, DateTime fulfilledAt)
    {
        Id = id;
        RemainingQuantity = remainingQuantity;
        TradePrice = tradePrice;
        FulfilledAt = fulfilledAt;
    }

    public Guid Id { get; }
    public int RemainingQuantity { get; }
    public decimal TradePrice { get; }
    public DateTime FulfilledAt { get; }
}
