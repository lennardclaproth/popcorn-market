using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Domain.Events;
public sealed record OrderCancelled : IDomainEvent
{
    public OrderCancelled(Guid orderId, string reason, DateTime cancelledAt)
    {
        OrderId = orderId;
        Reason = reason;
        CancelledAt = cancelledAt;
    }

    public Guid OrderId { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }

}
