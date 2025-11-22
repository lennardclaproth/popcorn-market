using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class FinancialStatement : Entity
{
    public string Ticker { get; private set; } = null!;
    public DateTime PublishDate { get; private set; }
    public ReportingPeriod ReportingPeriod { get; private set; } = null!;
    public int ComparablePeriod { get; private set; }
    public IncomeStatement IncomeStatement { get; private set; } = null!;
    public BalanceSheet BalanceSheet { get; private set; } = null!;
    public CashFlowStatement CashFlowStatement { get; private set; } = null!;

    private FinancialStatement() { }

    private FinancialStatement(string ticker, ReportingPeriod reportingPeriod, 
        IncomeStatement incomeStatement, BalanceSheet balanceSheet, 
        CashFlowStatement cashFlowStatement) : base(Guid.NewGuid())
    {
        Ticker = ticker;
        ReportingPeriod = reportingPeriod;
        ComparablePeriod = reportingPeriod.ToComparableValue();
        IncomeStatement = incomeStatement;
        BalanceSheet = balanceSheet;
        CashFlowStatement = cashFlowStatement;
        PublishDate = DateTime.Now;
    }

    public static Result<FinancialStatement> Create(string ticker, ReportingPeriod reportingPeriod,
        IncomeStatement incomeStatement, BalanceSheet balanceSheet, CashFlowStatement cashFlowStatement)
    {
        var financialStatement = new FinancialStatement(ticker, reportingPeriod, incomeStatement, balanceSheet,
            cashFlowStatement);
        
        return Result<FinancialStatement>.Success(financialStatement);
    }
}
