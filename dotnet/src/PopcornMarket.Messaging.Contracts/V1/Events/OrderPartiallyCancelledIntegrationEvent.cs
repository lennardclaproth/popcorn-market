using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.Messaging.Contracts.V1.Events;
public sealed record OrderPartiallyCancelledIntegrationEvent(OrderPartiallyCancelledPayload Payload) : IIntegrationEvent<OrderPartiallyCancelledPayload>
{
    public string Topic { get; } = TopicConstants.OrderPartiallyCancelled;
}

public sealed record OrderPartiallyCancelledPayload
{
    public OrderPartiallyCancelledPayload() { }
    [JsonPropertyName("order_id")]
    public Guid OrderId { get; init; }
    [JsonPropertyName("reason")]
    public required string Reason { get; init; } = null!;
    [JsonPropertyName("cancelled_at")]
    public DateTime CancelledAt { get; init; }
    [JsonPropertyName("remaining_quantity")]
    public int RemainingQuantity { get; init; }
}
