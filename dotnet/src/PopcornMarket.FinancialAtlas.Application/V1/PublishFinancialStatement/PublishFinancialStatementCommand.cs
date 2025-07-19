using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.FinancialAtlas.Contracts.Enums;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.PublishFinancialStatement;

public sealed record PublishFinancialStatementCommand : ICommand
{
    public string Ticker { get; init; } = null!;
    public int Year { get; init; }
    public PeriodType Interval { get; init; }
    public int PeriodNumber { get; set; }
    public IncomeStatementDto IncomeStatement { get; init; } = null!;
    public BalanceSheetDto BalanceSheet { get; init; } = null!;
    public CashFlowStatementDto CashFlowStatement { get; init; } = null!;
}
