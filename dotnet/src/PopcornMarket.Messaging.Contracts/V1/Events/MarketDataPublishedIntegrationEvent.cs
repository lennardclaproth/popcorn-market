using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.Messaging.Contracts.V1.Events;

public sealed record MarketDataPublishedIntegrationEvent(MarketDataPublishedPayload Payload) : IIntegrationEvent<MarketDataPublishedPayload>
{
    public string Topic { get; } = TopicConstants.MarketDataPublished;
    public MarketDataPublishedPayload Payload { get; } = Payload;
}

public sealed record MarketDataPublishedPayload
{
    [JsonPropertyName("ticker")]
    public required string Ticker { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("shares_outstanding")]
    public required long SharesOutstanding { get; set; }
    [JsonPropertyName("stock_price_USD")]
    public required decimal StockPriceUSD { get; set; }
    [JsonPropertyName("market_cap_B")]
    public decimal MarketCapBillion { get; init; }
    [JsonPropertyName("volume")]
    public long Volume { get; init; }
    [JsonPropertyName("date")]
    public DateTime Date { get; init; }
}
