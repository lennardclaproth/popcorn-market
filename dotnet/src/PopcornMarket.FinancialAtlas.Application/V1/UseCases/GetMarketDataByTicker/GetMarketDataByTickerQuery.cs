using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.GetMarketDataByTicker;

public sealed record GetMarketDataByTickerQuery : IQuery<MarketDataDto>
{
    public required string Ticker { get; init; }
}
