using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Errors;

public class CompanyErrors
{
    public static readonly Error CompanyAlreadyExists =
        Error.Conflict("Company.AlreadyExists", "The company with the supplied Ticker already exists.");
    
    public static readonly Error CompanyNotFound =
        Error.NotFound("Company.NotFound", "The company with the supplied Ticker was not found.");
}
