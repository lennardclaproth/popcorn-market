using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishMacroEconomicArticle;

internal sealed class PublishMacroEconomicArticleCommandHandler : ICommandHandler<PublishMacroEconomicArticleCommand, Guid>
{
    private readonly IMacroEconomicArticleRepository _macroEconomicArticleRepository;

    public PublishMacroEconomicArticleCommandHandler(IMacroEconomicArticleRepository macroEconomicArticleRepository)
    {
        _macroEconomicArticleRepository = macroEconomicArticleRepository;
    }

    public async Task<Result<Guid>> Handle(PublishMacroEconomicArticleCommand request, CancellationToken cancellationToken)
    {
        var creationResult = MacroEconomicArticle.Create(request.Headline, request.Content, request.Region);

        if (creationResult.IsFailure) return Result<Guid>.Failure(creationResult.Error);
        
        var article = Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));
        
        await _macroEconomicArticleRepository.Add(article);
        
        return Result<Guid>.Success(article.Id);
    }
}
