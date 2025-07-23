using System.Text.Json.Serialization;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public record GetFinancialStatementByTickerResponse
{
    [JsonPropertyName("financial_statement")]
    public FinancialStatementDto FinancialStatement { get; init; } = null!;
}
