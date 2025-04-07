using FastEndpoints;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetArticlesByTickerRequest
{
    public required string Ticker { get; init; }
    [QueryParam]
    public int Limit { get; init; }
}
