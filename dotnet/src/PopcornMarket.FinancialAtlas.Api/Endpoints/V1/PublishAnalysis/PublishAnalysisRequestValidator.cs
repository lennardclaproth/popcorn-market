using FluentValidation;
using PopcornMarket.FinancialAtlas.Contracts.Requests;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.PublishAnalysis;

public class PublishAnalysisRequestValidator : AbstractValidator<PublishAnalysisRequest>
{
    public PublishAnalysisRequestValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .WithMessage("Ticker is required.");

        RuleFor(x => x.Current)
            .NotNull()
            .ExclusiveBetween(-1f, 1f)
            .WithMessage("Current must be between -1 and 1 (exclusive) and not null.");

        RuleFor(x => x.OneWeek)
            .NotNull()
            .ExclusiveBetween(-1f, 1f)
            .WithMessage("OneWeek must be between -1 and 1 (exclusive) and not null.");

        RuleFor(x => x.OneMonth)
            .NotNull()
            .ExclusiveBetween(-1f, 1f)
            .WithMessage("OneMonth must be between -1 and 1 (exclusive) and not null.");

        RuleFor(x => x.ThreeMonths)
            .NotNull()
            .ExclusiveBetween(-1f, 1f)
            .WithMessage("ThreeMonths must be between -1 and 1 (exclusive) and not null.");


        RuleFor(x => x.TargetPrice)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Target price must be greater than 0.");
    }
}
