using Popcorn.FinancialAtlas.Domain.ValueObjects;
using Popcorn.FinancialAtlas.Domain.Enums;
using PopcornMarket.SharedKernel.Primitives;
using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Entities;

public class FinancialStatement : Entity
{
    public string TickerSymbol { get; private set; } = null!;
    public int Year { get; private set; }
    public FinancialStatementInterval StatementInterval { get; private set; }
    public IncomeStatement IncomeStatement { get; private set; } = null!;
    public BalanceSheet BalanceSheet { get; private set; } = null!;
    public CashFlowStatement CashFlowStatement { get; private set; } = null!;

    private FinancialStatement() { }

    private FinancialStatement(string tickerSymbol, int year, FinancialStatementInterval statementInterval, 
        IncomeStatement incomeStatement, BalanceSheet balanceSheet, 
        CashFlowStatement cashFlowStatement) : base(Guid.NewGuid())
    {
        TickerSymbol = tickerSymbol;
        Year = year;
        StatementInterval = statementInterval;
        IncomeStatement = incomeStatement;
        BalanceSheet = balanceSheet;
        CashFlowStatement = cashFlowStatement;
    }

    public static Result<FinancialStatement> Create(string tickerSymbol, int year, FinancialStatementInterval statementInterval,
        IncomeStatement incomeStatement, BalanceSheet balanceSheet, CashFlowStatement cashFlowStatement)
    {
        var financialStatement = new FinancialStatement(tickerSymbol, year, statementInterval, incomeStatement, balanceSheet,
            cashFlowStatement);
        
        return Result<FinancialStatement>.Success(financialStatement);
    }
}
