using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.GetCompanyByTicker;

public sealed record GetCompanyByTickerQuery : IQuery<CompanyDto>
{
    public required string Ticker { get; init; }
}
