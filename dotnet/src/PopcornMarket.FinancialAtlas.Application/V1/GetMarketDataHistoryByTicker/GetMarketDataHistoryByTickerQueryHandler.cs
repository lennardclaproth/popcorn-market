using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetMarketDataHistoryByTicker;

internal sealed class GetMarketDataHistoryByTickerQueryHandler : IQueryHandler<GetMarketDataHistoryByTickerQuery, IEnumerable<MarketSnapshotDto>>
{
    public Task<Result<IEnumerable<MarketSnapshotDto>>> Handle(GetMarketDataHistoryByTickerQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
