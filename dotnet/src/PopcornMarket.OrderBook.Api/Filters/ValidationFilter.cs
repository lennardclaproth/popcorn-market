using FluentValidation;

namespace PopcornMarket.OrderBook.Api.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationFilter(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_validator is not null)
        {
            var arg = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (arg is not null)
            {
                var validationResult = await _validator.ValidateAsync(arg, context.HttpContext.RequestAborted);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }
        }

        return await next(context);
    }
}
