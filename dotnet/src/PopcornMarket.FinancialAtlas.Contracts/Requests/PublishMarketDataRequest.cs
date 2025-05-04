using System.Text.Json.Serialization;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Contracts.Requests;

public record PublishMarketDataRequest
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = null!;
    [JsonPropertyName("shares_outstanding")]
    public long SharesOutstanding { get; init; }
    [JsonPropertyName("current")]
    public MarketSnapshotDto Current { get; init; } = null!;
    [JsonPropertyName("history")]
    public List<MarketSnapshotDto> History { get; init; } = new();
}
