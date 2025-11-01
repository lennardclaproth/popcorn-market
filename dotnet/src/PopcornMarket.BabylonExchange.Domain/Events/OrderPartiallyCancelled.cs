using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed record OrderPartiallyCancelled : IDomainEvent
{
    public OrderPartiallyCancelled(Guid orderId, string reason, int remainingQuantity, DateTime cancelledAt)
    {
        OrderId = orderId;
        Reason = reason;
        CancelledAt = cancelledAt;
        RemainingQuantity = remainingQuantity;
    }
    public OrderPartiallyCancelled(){}

    public Guid OrderId { get; init; }
    public required string Reason { get; init; } = null!;
    public DateTime CancelledAt { get; init; }
    public int RemainingQuantity { get; init; }
}
