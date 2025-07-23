using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public class FinancialStatementMappingProfile : Profile
{
    public FinancialStatementMappingProfile()
    {
        CreateMap<FinancialStatement, FinancialStatementDto>()
            .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.ReportingPeriod.ToString()));
        
        CreateMap<FinancialStatementDto, FinancialStatement>();
    }
}
