using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticles;

public sealed record GetMacroEconomicArticlesQuery : IQuery<IEnumerable<ArticleDto>>
{
    public int Limit { get; set; }
}
