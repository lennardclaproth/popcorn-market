using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesByRegion;

public sealed record GetArticlesByRegionQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required string Region { get; init; }
    public required int Limit { get; init; }
}
