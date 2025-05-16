using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Requests;

public sealed record GetCompanyByTickerRequest
{
    [JsonPropertyName("ticker")]
    public required string Ticker { get; init; }
}
