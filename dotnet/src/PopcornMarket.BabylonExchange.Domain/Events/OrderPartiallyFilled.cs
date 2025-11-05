using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed record OrderPartiallyFilled : IDomainEvent
{
    public OrderPartiallyFilled(Guid orderId, int orderRemainingQuantity, decimal tradePrice, DateTime utcNow)
    {
        Id=orderId;
        RemainingQuantity=orderRemainingQuantity;
        TradePrice=tradePrice;
        FulfilledAt = utcNow;
    }

    public OrderPartiallyFilled() { }

    public Guid Id { get; init; }
    public int RemainingQuantity { get; init; }
    public decimal TradePrice { get; init; }
    public DateTime FulfilledAt { get; init; }
}
