using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesBySector;

public sealed record GetArticlesBySectorQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required string Sector { get; init; }
    public required int Limit { get; init; }
}
