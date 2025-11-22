using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.PublishAnalysis;
internal sealed class PublishAnalysisCommandHandler : ICommandHandler<PublishAnalysisCommand>
{
    private readonly IMarketDataRepository _marketDataRepository;

    public PublishAnalysisCommandHandler(IMarketDataRepository marketDataRepository)
    {
        _marketDataRepository = marketDataRepository;
    }

    public async Task<Result> Handle(PublishAnalysisCommand request, CancellationToken cancellationToken)
    {
        var analysis = new Analysis(request.Ticker,
            request.Current,
            request.OneWeek,
            request.OneMonth,
            request.ThreeMonths,
            request.TargetPrice,
            DateTime.Now);

        await _marketDataRepository.InsertAnalysis(analysis);
        return Result.Success();
    }
}
