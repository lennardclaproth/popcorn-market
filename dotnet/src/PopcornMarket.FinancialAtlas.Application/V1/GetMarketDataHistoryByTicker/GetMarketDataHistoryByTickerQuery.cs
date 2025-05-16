using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetMarketDataHistoryByTicker;

public sealed record GetMarketDataHistoryByTickerQuery : IQuery<IEnumerable<MarketSnapshotDto>>
{
    public required string Ticker { get; init; }
    public string Range { get; init; } = "YTD";
}
