using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public sealed record GetTickersResponse
{
    [JsonPropertyName("tickers")]
    public required IEnumerable<string> Tickers { get; init; }
}
