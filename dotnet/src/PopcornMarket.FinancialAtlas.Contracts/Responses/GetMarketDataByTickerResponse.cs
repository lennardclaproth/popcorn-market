using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public sealed record GetMarketDataByTickerResponse
{
    public required MarketSnapshotDto MarketSnapshot { get; init; }
}
