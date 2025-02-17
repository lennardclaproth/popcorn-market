using Popcorn.FinancialAtlas.Domain.ValueObjects;
using Popcorn.FinancialAtlas.Domain.Enums;
using PopcornMarket.SharedKernel.Primitives;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class FinancialStatement : Entity
{
    public string TickerSymbol { get; private set; } = null!;
    public int Year { get; private set; }
    public FinancialInterval Interval { get; private set; }
    public IncomeStatement IncomeStatement { get; private set; } = null!;
    public BalanceSheet BalanceSheet { get; private set; } = null!;
    public CashFlowStatement CashFlowStatement { get; private set; } = null!;

    private FinancialStatement() { }

    public FinancialStatement(string tickerSymbol, int year, FinancialInterval interval, 
        IncomeStatement incomeStatement, BalanceSheet balanceSheet, 
        CashFlowStatement cashFlowStatement) : base(Guid.NewGuid())
    {
        TickerSymbol = tickerSymbol;
        Year = year;
        Interval = interval;
        IncomeStatement = incomeStatement;
        BalanceSheet = balanceSheet;
        CashFlowStatement = cashFlowStatement;
    }
}
