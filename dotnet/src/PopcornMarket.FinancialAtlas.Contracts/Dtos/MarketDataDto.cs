using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record MarketDataDto()
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("ticker")] 
    public string Ticker { get; init; } = null!;
    [JsonPropertyName("shares_outstanding")]
    public long SharesOutstanding { get; init; }
    [JsonPropertyName("market_snapshot")]
    public MarketSnapshotDto MarketSnapshot { get; init; } = null!;
};
