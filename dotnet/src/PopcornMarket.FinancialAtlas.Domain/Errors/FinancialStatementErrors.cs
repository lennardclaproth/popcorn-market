using PopcornMarket.SharedKernel.ResultPattern;

namespace Popcorn.FinancialAtlas.Domain.Errors;

public static class FinancialStatementErrors
{
    public static readonly Error ReportingPeriodTooSmall =
        Error.Validation("FinancialStatement.ReportingPeriodTooSmall", "The reporting period for the company cannot be smaller than an already existing financial statement.");
    
    public static readonly Error FinancialStatementNotFound =
        Error.NotFound("FinancialStatement.NotFound", "The financial statement for the company with the supplied Ticker was not found.");
}
