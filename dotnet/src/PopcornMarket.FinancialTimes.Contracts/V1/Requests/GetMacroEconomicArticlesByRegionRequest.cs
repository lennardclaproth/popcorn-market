namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetMacroEconomicArticlesByRegionRequest
{
    public required string Region { get; init; }
    public required int Limit { get; init; }
}
