using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishSectorArticle;

internal sealed class PublishSectorArticleCommandHandler : ICommandHandler<PublishSectorArticleCommand>
{
    private readonly ISectorArticleRepository _sectorArticleRepository;

    public PublishSectorArticleCommandHandler(ISectorArticleRepository sectorArticleRepository)
    {
        _sectorArticleRepository = sectorArticleRepository;
    }

    public async Task<Result> Handle(PublishSectorArticleCommand request, CancellationToken cancellationToken)
    {
        var creationResult = SectorArticle.Create(request.Headline, request.Content, request.Sector);

        if (creationResult.IsFailure) return creationResult;
        
        var article = Guard.Against.Null(creationResult.Value, nameof(SectorArticle));
        
        await _sectorArticleRepository.Add(article);
        
        return Result.Success();
    }
}
