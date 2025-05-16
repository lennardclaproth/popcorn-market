using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public sealed record GetTickersResponse
{
    [JsonPropertyName("tickers")]
    public required IEnumerable<string> Tickers { get; init; }
}
