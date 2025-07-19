using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record BalanceSheetDto
{
    [JsonPropertyName("total_assets_B")]
    public decimal TotalAssetsB { get; init; }
    
    [JsonPropertyName("total_liabilities_B")]
    public decimal TotalLiabilitiesB { get; init; }
    
    [JsonPropertyName("total_equity_B")]
    public decimal TotalEquityB { get; init; }
    
    [JsonPropertyName("debt_to_equity_ratio")]
    public decimal DebtToEquityRatio { get; init; }
}
