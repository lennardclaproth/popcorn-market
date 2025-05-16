using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Contracts.Responses;

public sealed record GetCompanyByTickerResponse
{
    public required CompanyDto Company { get; init; }
}
