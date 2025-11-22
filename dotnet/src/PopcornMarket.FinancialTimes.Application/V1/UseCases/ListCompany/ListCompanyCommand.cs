using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.Application.V1.UseCases.ListCompany;
public sealed record ListCompanyCommand : ICommand
{
    public required string Ticker { get; init; }
}
