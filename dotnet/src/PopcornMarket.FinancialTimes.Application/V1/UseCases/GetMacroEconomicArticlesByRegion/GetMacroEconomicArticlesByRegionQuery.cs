using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.UseCases.GetMacroEconomicArticlesByRegion;

public sealed record GetMacroEconomicArticlesByRegionQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required string Region { get; init; }
    public required int Limit { get; init; }
}
