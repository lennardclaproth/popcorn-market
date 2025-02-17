namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record CashFlowStatement(
    decimal OperatingCashFlowB,
    decimal CapitalExpendituresB,
    decimal FreeCashFlowB,
    decimal FinancingCashFlowB,
    decimal InvestingCashFlowB,
    decimal NetCashFlowB);
