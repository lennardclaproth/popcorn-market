﻿using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishPoliticalArticle;

internal sealed class PublishPoliticalArticleCommandHandler : ICommandHandler<PublishPoliticalArticleCommand, Guid>
{
    private readonly IPoliticalArticleRepository _politicalArticleRepository;

    public PublishPoliticalArticleCommandHandler(IPoliticalArticleRepository politicalArticleRepository)
    {
        _politicalArticleRepository = politicalArticleRepository;
    }

    public async Task<Result<Guid>> Handle(PublishPoliticalArticleCommand request, CancellationToken cancellationToken)
    {
        var creationResult = PoliticalArticle.Create(request.Headline, request.Content, request.Region);
        
        if(creationResult.IsFailure) return Result<Guid>.Failure(creationResult.Error);
        
        var article = Guard.Against.Null(creationResult.Value, nameof(PoliticalArticle));
        
        await _politicalArticleRepository.Add(article);
        
        return Result<Guid>.Success(article.Id);
    }
}
