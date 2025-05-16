using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetMarketDataByTicker;

public sealed record GetMarketDataByTickerQuery : IQuery<MarketSnapshotDto>
{
    public required string Ticker { get; init; }
}
