using Ardalis.GuardClauses;
using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.Errors;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.PublishMarketData;

internal sealed class PublishMarketDataCommandHandler : ICommandHandler<PublishMarketDataCommand>
{
    private readonly IMapper _mapper;
    private readonly IMarketDataRepository _marketDataRepository;
    private readonly ICompanyRepository _companyRepository;

    public PublishMarketDataCommandHandler(IMapper mapper, IMarketDataRepository marketDataRepository, ICompanyRepository companyRepository)
    {
        _mapper = mapper;
        _marketDataRepository = marketDataRepository;
        _companyRepository = companyRepository;
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
        var company = await _companyRepository.GetByTicker(request.Ticker);
        var existingMarketData = await _marketDataRepository.GetByTicker(request.Ticker);

        if (company == null) return Result.Failure(CompanyErrors.CompanyNotFound);
        
        if (existingMarketData != null) return Result.Failure(MarketDataErrors.MarketDataAlreadyExists);
        
        var currentSnapshot = _mapper.Map<MarketSnapshot>(request.Current);
        var history = new List<MarketHistory>();
        foreach (var item in request.History)
        {
            var a = new MarketHistory(request.Ticker, item.StockPriceUSD, item.MarketCapBillion, item.Volume, item.DividendPerShareUSD, item.DividendYieldPercent, item.Date);
            history.Add(a);
        }

        var creationResult = MarketData.Create(request.Ticker, request.SharesOutstanding, currentSnapshot, history);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value), "Value cannot be null here.");
        
        await _marketDataRepository.Add(creationResult.Value);
        await _marketDataRepository.InsertHistory(creationResult.Value.History, cancellationToken);

        //var marketData = creationResult.Value;

        //var marketDataPublishedEvent = new MarketDataPublishedPayload
        //{
        //    Ticker = company.Ticker,
        //    Name = company.Name,
        //    Date = marketData.Current.Date,
        //    MarketCapBillion = marketData.Current.MarketCapBillion,
        //    SharesOutstanding = marketData.SharesOutstanding,
        //    StockPriceUSD = marketData.Current.StockPriceUSD,
        //    Volume = marketData.Current.Volume
        //};
        //await _producer.Produce(TopicConstants.MarketDataPublished, marketDataPublishedEvent, cancellationToken);
        
        return Result.Success();
    }
}
