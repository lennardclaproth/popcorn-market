using FastEndpoints;
using FluentValidation;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetPoliticalArticles;

internal sealed class GetPoliticalArticlesRequestValidator : Validator<GetPoliticalArticlesRequest>
{
    public GetPoliticalArticlesRequestValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}
