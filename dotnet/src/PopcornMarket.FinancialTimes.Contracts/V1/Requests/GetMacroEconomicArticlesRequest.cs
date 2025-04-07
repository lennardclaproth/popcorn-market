using FastEndpoints;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetMacroEconomicArticlesRequest
{
    [QueryParam]
    public int Limit { get; init; }
}
