using System.Text.Json.Serialization;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public sealed record GetMarketDataByTickerResponse
{
    [JsonPropertyName("market_data")]
    public MarketDataDto MarketData { get; init; } = null!;
}
