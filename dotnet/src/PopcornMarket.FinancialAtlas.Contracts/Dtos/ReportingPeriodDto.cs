using System.Text.Json.Serialization;
using PopcornMarket.FinancialAtlas.Contracts.Enums;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record ReportingPeriodDto
{
    [JsonPropertyName("year")]
    public int Year { get; }
    [JsonPropertyName("period_type")]
    public PeriodType Type { get; }
    [JsonPropertyName("period_number")]
    public int PeriodNumber { get; }
}
