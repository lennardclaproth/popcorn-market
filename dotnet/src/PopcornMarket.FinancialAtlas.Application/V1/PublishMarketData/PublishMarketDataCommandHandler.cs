using Ardalis.GuardClauses;
using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.Errors;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.PublishMarketData;

internal sealed class PublishMarketDataCommandHandler : ICommandHandler<PublishMarketDataCommand>
{
    private readonly IMapper _mapper;
    private readonly IMarketDataRepository _marketDataRepository;

    public PublishMarketDataCommandHandler(IMapper mapper, IMarketDataRepository marketDataRepository)
    {
        _mapper = mapper;
        _marketDataRepository = marketDataRepository;
    }

    /// <summary>
    /// Handles the creation of new market data, checks
    /// if it already exists under that ticker. If it already
    /// exists it should return a conflict error.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result> Handle(PublishMarketDataCommand request, CancellationToken cancellationToken)
    {
        var existingMarketData = await _marketDataRepository.GetByTicker(request.Ticker);

        if (existingMarketData != null)
        {
            return Result.Failure(MarketDataErrors.MarketDataAlreadyExists);
        }
        
        var currentSnapshot = _mapper.Map<MarketSnapshot>(request.Current);
        var history = _mapper.Map<List<MarketSnapshot>>(request.History);
        
        var creationResult = MarketData.Create(request.Ticker, request.SharesOutstanding, currentSnapshot, history);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value), "Value cannot be null here.");
        
        await _marketDataRepository.Add(creationResult.Value);
        return Result.Success();
    }
}
