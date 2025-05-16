using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticlesByRegion;

public sealed record GetPoliticalArticlesByRegionQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required string Region { get; init; }
    public required int Limit { get; init; }
}
