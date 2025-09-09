using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed class OrderPartiallyCancelled : IDomainEvent
{
    public OrderPartiallyCancelled(Guid orderId, string reason, int remainingQuantity, DateTime cancelledAt)
    {
        OrderId = orderId;
        Reason = reason;
        CancelledAt = cancelledAt;
        RemainingQuantity = remainingQuantity;
    }

    public Guid OrderId { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }
    public int RemainingQuantity { get; }
}
