using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;
public record AnalysisDto
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = null!;
    [JsonPropertyName("current")]
    public float Current { get; init; }
    [JsonPropertyName("1w")]
    public float OneWeek { get; init; }
    [JsonPropertyName("1m")]
    public float OneMonth { get; init; }
    [JsonPropertyName("3m")]
    public float ThreeMonths { get; init; }
    [JsonPropertyName("target_price")]
    public decimal TargetPrice { get; init; }
    [JsonPropertyName("date")]
    public DateTime Date { get; init; }
}
