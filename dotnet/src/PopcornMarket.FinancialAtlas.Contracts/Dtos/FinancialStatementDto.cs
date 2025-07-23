using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public record FinancialStatementDto
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = string.Empty;
    [JsonPropertyName("publish_date")]
    public DateTime PublishDate { get; private set; }
    [JsonPropertyName("period")]
    public string Period { get; init; } = null!;
    [JsonPropertyName("income_statement")]
    public IncomeStatementDto IncomeStatement { get; init; } = new();
    [JsonPropertyName("balance_sheet")]
    public BalanceSheetDto BalanceSheet { get; init; } = new();
    [JsonPropertyName("cash_flow_statement")]
    public CashFlowStatementDto CashFlowStatement { get; init; } = new();
    
}
