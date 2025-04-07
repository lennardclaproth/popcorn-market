using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.PublishCompanyArticle;

internal sealed class PublishCompanyArticleCommandHandler : ICommandHandler<PublishCompanyArticleCommand>
{
    private readonly ICompanyArticleRepository _companyArticleRepository;

    public PublishCompanyArticleCommandHandler(ICompanyArticleRepository companyArticleRepository)
    {
        _companyArticleRepository = companyArticleRepository;
    }

    public async Task<Result> Handle(PublishCompanyArticleCommand request, CancellationToken cancellationToken)
    {
        var articleCreationResult = CompanyArticle.Create(request.Headline, request.Content, request.TickerSymbol, request.Sector,
            request.CompanyName);

        if (articleCreationResult.IsFailure)
        {
            return articleCreationResult;
        }

        Guard.Against.Null(articleCreationResult.Value, nameof(articleCreationResult.Value));
        
        await _companyArticleRepository.Add(articleCreationResult.Value);
        
        return Result.Success();
    }
}
