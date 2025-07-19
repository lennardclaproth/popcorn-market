using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public class FinancialStatementMappingProfile : Profile
{
    public FinancialStatementMappingProfile()
    {
        CreateMap<FinancialStatement, FinancialStatementDto>();
        CreateMap<FinancialStatementDto, FinancialStatement>();
    }
}
