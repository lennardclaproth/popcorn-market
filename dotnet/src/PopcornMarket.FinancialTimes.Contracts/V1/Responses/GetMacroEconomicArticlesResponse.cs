using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;

namespace PopcornMarket.FinancialTimes.Contracts.V1.Responses;

public sealed record GetMacroEconomicArticlesResponse
{
    public required IEnumerable<ArticleDto> Articles { get; init; }
}
