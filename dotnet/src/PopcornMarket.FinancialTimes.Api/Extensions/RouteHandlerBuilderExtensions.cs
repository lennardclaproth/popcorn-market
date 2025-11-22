using Elastic.Apm;
using PopcornMarket.FinancialTimes.Api.Filters;

namespace PopcornMarket.FinancialTimes.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder AddValidation<TRequest>(this RouteHandlerBuilder builder) where TRequest : class
    {
        builder.AddEndpointFilter<ValidationFilter<TRequest>>();
        return builder;
    }

    public static RouteHandlerBuilder WithTransactionName(
        this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder =>
        {
            if (endpointBuilder is RouteEndpointBuilder routeEndpointBuilder)
            {
                var routePattern = routeEndpointBuilder.RoutePattern.RawText;

                if (!string.IsNullOrWhiteSpace(routePattern))
                {
                    routeEndpointBuilder.FilterFactories.Add((context, next) =>
                    {
                        return async invocationContext =>
                        {
                            var method = invocationContext.HttpContext.Request.Method;

                            var transaction = Agent.Tracer.CurrentTransaction;
                            if (transaction != null)
                            {
                                transaction.Name = $"{method} {routePattern}";
                            }

                            return await next(invocationContext);
                        };
                    });
                }
            }
        });

        return builder;
    }
}
