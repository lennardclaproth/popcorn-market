using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Enums;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public class PeriodTypeMappingProfile : Profile
{
    public PeriodTypeMappingProfile()
    {
        CreateMap<Contracts.Enums.PeriodType, PeriodType>();
        CreateMap<PeriodType, Contracts.Enums.PeriodType>();
    }
}
