using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.FinancialAtlas.Contracts.Enums;

namespace PopcornMarket.FinancialAtlas.Contracts.Requests;

public record PublishFinancialStatementRequest()
{
    public string Ticker { get; init; } = null!;
    public int Year { get; init; }
    public PeriodType Interval { get; init; }
    public int PeriodNumber { get; set; }
    public IncomeStatementDto IncomeStatement { get; init; } = null!;
    public BalanceSheetDto BalanceSheet { get; init; } = null!;
    public CashFlowStatementDto CashFlowStatement { get; init; } = null!;
};
