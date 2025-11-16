using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.Messaging.Contracts.V1.Events;
public sealed record OrderFulfilledIntegrationEvent(OrderFulfilledPayload Payload) : IIntegrationEvent<OrderFulfilledPayload>
{
    public string Topic { get; } = TopicConstants.OrderFulfilled;
}

public sealed record OrderFulfilledPayload
{
    public OrderFulfilledPayload() { }
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    [JsonPropertyName("trade_price")]
    public decimal TradePrice { get; init; }
    [JsonPropertyName("trade_quantity")]
    public int TradeQuantity { get; init; }
    [JsonPropertyName("fulfilled_at")]
    public DateTime FulfilledAt { get; init; }
}
