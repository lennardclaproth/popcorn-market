using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public record GetArticlesByTickerResponse
{
    public required string Ticker { get; init; }
    public required IEnumerable<ArticleDto> Articles { get; init; }
}
