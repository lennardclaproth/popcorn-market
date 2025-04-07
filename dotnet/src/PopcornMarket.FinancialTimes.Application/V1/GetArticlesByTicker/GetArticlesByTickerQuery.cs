using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesByTicker;

public sealed record GetArticlesByTickerQuery : IQuery<IEnumerable<ArticleDto>>
{
    public required string Ticker { get; init; }
    public required int Limit { get; init; }
}
