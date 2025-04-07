using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticles;

public sealed record GetPoliticalArticlesQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required int Limit { get; init; }
}
