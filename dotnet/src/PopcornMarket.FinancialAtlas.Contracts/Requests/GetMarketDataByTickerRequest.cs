using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetMarketDataByTicker;

internal sealed record GetMarketDataByTickerRequest : IQuery<MarketSnapshotDto>
{
    public required string Ticker { get; init; }
}
