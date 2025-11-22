using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.Messaging.Contracts.V1.Events;

public sealed record TradeExecutedIntegrationEvent(TradeExecutedPayload Payload) : IIntegrationEvent<TradeExecutedPayload>
{
    public string Topic { get; } = TopicConstants.TradeExecuted;
}

public sealed record TradeExecutedPayload
{
    public TradeExecutedPayload() { }
    public Guid BuyOrderId { get; init; }
    public Guid SellOrderId { get; init; }
    public string StockSymbol { get; init; } = null!;
    public decimal TradePrice { get; init; }
    public int TradeQuantity { get; init; }
    public DateTime ExecutedAt { get; init; }
}
