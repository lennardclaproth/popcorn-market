using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record IncomeStatementDto
{
    [JsonPropertyName("revenue_B")]
    public decimal RevenueB { get; init; }
    [JsonPropertyName("cogs_B")]
    public decimal CogsB { get; init; }
    [JsonPropertyName("gross_profit_B")]
    public decimal GrossProfitB { get; init; }
    [JsonPropertyName("operating_expenses_B")]
    public decimal OperatingExpensesB { get; init; }
    [JsonPropertyName("ebitda_B")]
    public decimal EbitdaB { get; init; }
    [JsonPropertyName("depreciation_amortization_B")]
    public decimal DepreciationAmortizationB { get; init; }
    [JsonPropertyName("ebit_B")]
    public decimal EbitB { get; init; }
    [JsonPropertyName("interest_expense_B")]
    public decimal InterestExpenseB { get; init; }
    [JsonPropertyName("tax_rate_percent")]
    public decimal TaxRatePercent { get; init; }
    [JsonPropertyName("net_income_B")]
    public decimal NetIncomeB { get; init; }
    [JsonPropertyName("eps_USD")]
    public decimal EpsUSD { get; init; }
}
