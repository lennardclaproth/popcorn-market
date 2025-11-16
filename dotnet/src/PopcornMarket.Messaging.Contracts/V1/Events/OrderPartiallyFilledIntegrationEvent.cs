using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.Messaging.Contracts.V1.Events;
public sealed record OrderPartiallyFilledIntegrationEvent(OrderPartiallyFilledPayload Payload) : IIntegrationEvent<OrderPartiallyFilledPayload>
{
    public string Topic { get; } = TopicConstants.OrderPartiallyFilled;
    public OrderPartiallyFilledPayload Payload { get; } = Payload;
}

public sealed record OrderPartiallyFilledPayload
{
    public OrderPartiallyFilledPayload() {}
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    [JsonPropertyName("remaining_quantity")]
    public int RemainingQuantity { get; init; }
    [JsonPropertyName("trade_price")]
    public decimal TradePrice { get; init; }
    [JsonPropertyName("fulfilled_at")]
    public DateTime FulfilledAt { get; init; }
}
