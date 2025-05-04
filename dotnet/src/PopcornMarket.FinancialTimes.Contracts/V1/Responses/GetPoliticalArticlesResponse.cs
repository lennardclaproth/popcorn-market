using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public sealed record GetPoliticalArticlesResponse
{
    public required IEnumerable<ArticleDto> Articles { get; init; }
}
