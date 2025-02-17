namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record IncomeStatement(
    decimal RevenueB,
    decimal CogsB,
    decimal GrossProfitB,
    decimal OperatingExpensesB,
    decimal EbitdaB,
    decimal DepreciationAmortizationB,
    decimal EbitB,
    decimal InterestExpenseB,
    decimal TaxRatePercent,
    decimal NetIncomeB,
    decimal EpsUSD);
