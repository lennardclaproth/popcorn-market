using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Domain.Errors;

public static class CompanyErrors
{
    public static readonly Error CompanyAlreadyExists =
        Error.Conflict("Company.AlreadyExists", "The company with the supplied Ticker already exists.");
    
    public static readonly Error CompanyNotFound =
        Error.NotFound("Company.NotFound", "The company with the supplied Ticker was not found.");
}
