using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetCompanyByTicker;

internal sealed record GetCompanyByTickerQuery : IQuery<CompanyDto>
{
    public required string Ticker { get; init; }
}
