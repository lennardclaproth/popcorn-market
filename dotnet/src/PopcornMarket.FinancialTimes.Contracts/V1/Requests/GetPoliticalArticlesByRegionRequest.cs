namespace PopcornMarket.FinancialTimes.Contracts.V1.Requests;

public sealed record GetPoliticalArticlesByRegionRequest
{
    public required string Region { get; init; }
    public required int Limit { get; init; }
}
