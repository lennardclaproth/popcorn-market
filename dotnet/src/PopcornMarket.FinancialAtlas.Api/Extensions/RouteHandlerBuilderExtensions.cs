using PopcornMarket.FinancialAtlas.Api.Filters;

namespace PopcornMarket.FinancialAtlas.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder AddValidation<TRequest>(this RouteHandlerBuilder builder) where TRequest : class
    {
        builder.AddEndpointFilter<ValidationFilter<TRequest>>();
        return builder;
    }
}
