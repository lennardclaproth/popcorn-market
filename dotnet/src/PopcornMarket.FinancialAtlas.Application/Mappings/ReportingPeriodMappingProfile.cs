using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public class ReportingPeriodMappingProfile : Profile
{
    public ReportingPeriodMappingProfile()
    {
        CreateMap<ReportingPeriod, ReportingPeriodDto>();
        CreateMap<ReportingPeriodDto, ReportingPeriod>();
    }
}
