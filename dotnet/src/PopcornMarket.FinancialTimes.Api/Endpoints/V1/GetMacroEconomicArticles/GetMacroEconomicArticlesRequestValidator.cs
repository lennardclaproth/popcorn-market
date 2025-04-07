using FastEndpoints;
using FluentValidation;
using PopcornMarket.FinancialTimes.Contracts.V1.Requests;

namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetMacroEconomicArticles;

internal sealed class GetMacroEconomicArticlesRequestValidator : Validator<GetMacroEconomicArticlesRequest>
{
    public GetMacroEconomicArticlesRequestValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }    
}
