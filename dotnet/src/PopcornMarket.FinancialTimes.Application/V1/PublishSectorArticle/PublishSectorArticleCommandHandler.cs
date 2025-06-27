using Ardalis.GuardClauses;
using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishSectorArticle;

internal sealed class PublishSectorArticleCommandHandler : ICommandHandler<PublishSectorArticleCommand, Guid>
{
    private readonly ISectorArticleRepository _sectorArticleRepository;
    private readonly IMapper _mapper;

    public PublishSectorArticleCommandHandler(ISectorArticleRepository sectorArticleRepository, IMapper mapper)
    {
        _sectorArticleRepository = sectorArticleRepository;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(PublishSectorArticleCommand request, CancellationToken cancellationToken)
    {
        var creationResult = SectorArticle.Create(request.Headline, request.Content, request.Sector, request.Region);

        if (creationResult.IsFailure) return Result<Guid>.Failure(creationResult.Error);
        
        var article = Guard.Against.Null(creationResult.Value, nameof(SectorArticle));
        
        await _sectorArticleRepository.Add(article);
        
        return Result<Guid>.Success(article.Id);
    }
}
