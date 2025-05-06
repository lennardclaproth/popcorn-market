using PopcornMarket.OrderBook.Api.Filters;

namespace PopcornMarket.OrderBook.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder AddValidation<TRequest>(this RouteHandlerBuilder builder) where TRequest : class
    {
        builder.AddEndpointFilter<ValidationFilter<TRequest>>();
        return builder;
    }
}
