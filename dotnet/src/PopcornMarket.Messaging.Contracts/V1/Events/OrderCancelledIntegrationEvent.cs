using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.Messaging.Contracts.V1.Events;
public sealed record OrderCancelledIntegrationEvent(OrderCancelledPayload Payload) : IIntegrationEvent<OrderCancelledPayload>
{
    public string Topic { get; } = TopicConstants.OrderCancelled;
}

public sealed record OrderCancelledPayload
{
    public OrderCancelledPayload(){}
    [JsonPropertyName("order_id")]
    public Guid OrderId { get; init; }
    [JsonPropertyName("reason")]
    public string Reason { get; init; } = null!;
    [JsonPropertyName("cancelled_at")]
    public DateTime CancelledAt { get; init; }
}
