using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Errors;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.GetMarketDataByTicker;

internal sealed class GetMarketDataByTickerQueryHandler : IQueryHandler<GetMarketDataByTickerQuery, MarketDataDto>
{
    private readonly IMarketDataRepository _marketDataRepository;
    private readonly IMapper _mapper;

    public GetMarketDataByTickerQueryHandler(IMarketDataRepository marketDataRepository, IMapper mapper)
    {
        _marketDataRepository = marketDataRepository;
        _mapper = mapper;
    }

    public async Task<Result<MarketDataDto>> Handle(GetMarketDataByTickerQuery request, CancellationToken cancellationToken)
    {
        var marketData = await _marketDataRepository.GetByTicker(request.Ticker);
        var analysis = await _marketDataRepository.GetLatestAnalysisByTicker(request.Ticker);

        if (marketData == null) return Result<MarketDataDto>.Failure(MarketDataErrors.MarketDataNotFound);

        if (analysis != null)
        {
            marketData.HydrateAnalysis(analysis);
        }

        return Result<MarketDataDto>.Success(_mapper.Map<MarketDataDto>(marketData));
    }
}
