
namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetMacroEconomicArticlesRequest
{
    public int Limit { get; init; }
}
