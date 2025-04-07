using FluentValidation;

using PopcornMarket.FinancialTimes.Contracts.V1.Requests;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesBySector;

public class GetArticlesBySectorRequestValidator : AbstractValidator<GetArticlesBySectorRequest>
{
    public GetArticlesBySectorRequestValidator()
    {
        RuleFor(x => x.Sector)
            .Must(c => c.Length >= 3 && c.Length <= 255)
            .WithMessage("Sector must be between 3 and 255 characters");

        RuleFor(x => x.Limit)
            .Must(l => l is > 0 and <= 100)
            .WithMessage("Limit must be between 0 and 100");
    }
}
