namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetArticlesBySectorRequest
{
    public required string Sector { get; init; }
    public int Limit { get; init; }
}
