using PopcornMarket.SharedKernel.ResultPattern;
using PopcornMarket.SharedKernel.ResultPattern.Constants;

namespace PopcornMarket.OrderBook.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }
    
        return Results.Problem(
            statusCode: GetStatusCode(result.Error.Type),
            title: GetTitle(result.Error.Type),
            type: GetType(result.Error.Type),
            extensions: new Dictionary<string, object?>
            {
                { "errors", new[] { result.Error } }
            }
        );
    }

    static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => ErrorResultTitlesConstants.Validation,
            ErrorType.NotFound => ErrorResultTitlesConstants.NotFound,
            ErrorType.Conflict => ErrorResultTitlesConstants.Conflict,
            _ => ErrorResultTitlesConstants.ServerError
        };
    }
    
    static string GetType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => ErrorResultTypesConstants.Validation,
            ErrorType.NotFound => ErrorResultTypesConstants.NotFound,
            ErrorType.Conflict => ErrorResultTypesConstants.Conflict,
            _ => ErrorResultTypesConstants.ServerError
        };
    }
}
