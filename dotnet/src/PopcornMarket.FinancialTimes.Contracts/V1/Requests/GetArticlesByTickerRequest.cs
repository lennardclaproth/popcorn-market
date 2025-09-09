namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetArticlesByTickerRequest
{
    public required string Ticker { get; init; }
    public int Limit { get; init; }
}
