using FastEndpoints;
using FluentValidation;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesByTicker;

public class GetArticlesByTickerRequestValidator : Validator<GetArticlesByTickerRequest>
{
    public GetArticlesByTickerRequestValidator()
    {
        RuleFor(x => x.Ticker)
            .Must(t => t == null || t.Length is >= 3 and <= 6)
            .WithMessage("Ticker must be between 3 and 6 characters long");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}
