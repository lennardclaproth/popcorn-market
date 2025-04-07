namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetPoliticalArticlesRequest
{
    public int Limit { get; init; }
}
