using System.Text.Json.Serialization;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.FinancialAtlas.Contracts.Enums;

namespace PopcornMarket.FinancialAtlas.Contracts.Requests;

public record PublishFinancialStatementRequest()
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = null!;
    [JsonPropertyName("year")]
    public int Year { get; init; }
    [JsonPropertyName("internal")]
    public PeriodType Interval { get; init; }
    [JsonPropertyName("period_number")]
    public int PeriodNumber { get; set; }
    [JsonPropertyName("income_statement")]
    public IncomeStatementDto IncomeStatement { get; init; } = null!;
    [JsonPropertyName("balance_sheet")]
    public BalanceSheetDto BalanceSheet { get; init; } = null!;
    [JsonPropertyName("cash_flow_statement")]
    public CashFlowStatementDto CashFlowStatement { get; init; } = null!;
};
