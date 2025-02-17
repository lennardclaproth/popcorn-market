namespace Popcorn.FinancialAtlas.Domain.ValueObjects;

public record BalanceSheet(
    decimal TotalAssetsB,
    decimal TotalLiabilitiesB,
    decimal TotalEquityB,
    decimal DebtToEquityRatio);
