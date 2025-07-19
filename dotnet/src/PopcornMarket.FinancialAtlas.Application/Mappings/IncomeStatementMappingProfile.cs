using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public class IncomeStatementMappingProfile : Profile
{
    public IncomeStatementMappingProfile()
    {
        CreateMap<IncomeStatementDto, IncomeStatement>();
        CreateMap<IncomeStatement, IncomeStatementDto>();
    }
}
