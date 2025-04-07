using FluentValidation;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.PublishArticle;

public class PublishArticleRequestValidator : AbstractValidator<PublishArticleRequest>
{
    public PublishArticleRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .Must(c => c == null || c.Length < 3)
            .WithMessage("CompanyName must be at least 3 characters long");
        
        RuleFor(x => x.Industry)
            .Must(i => i == null || i.Length < 3)
            .WithMessage("Industry must be at least 3 characters long");
        
        RuleFor(x => x.Region)
            .Must(r => r == null || r.Length < 3)
            .WithMessage("Region must be at least 3 characters long");
        
        RuleFor(x => x.Ticker)
            .Must(t => t == null || t.Length is > 3 and < 6)
            .WithMessage("Ticker must be between 3 and 6 characters long");
        
        RuleFor(x => x.Sector)
            .Must(s => s == null || s?.Length > 3)
            .WithMessage("Sector must be at least 3 characters long");
    }
}
