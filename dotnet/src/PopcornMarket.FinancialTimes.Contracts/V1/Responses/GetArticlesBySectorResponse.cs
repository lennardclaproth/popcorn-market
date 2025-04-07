using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public sealed record GetArticlesBySectorResponse
{
    public required string Sector { get; init; }
    public required IEnumerable<ArticleDto> Articles { get; init; }
}
