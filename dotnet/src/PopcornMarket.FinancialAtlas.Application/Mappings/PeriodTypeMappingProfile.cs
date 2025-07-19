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
    
    // private static PeriodType MapToDomain(Contracts.Enums.PeriodType dto) =>
    // dto switch
    // {
    //     Contracts.Enums.PeriodType.Yearly => PeriodType.Yearly,
    //     Contracts.Enums.PeriodType.HalfYearly => PeriodType.HalfYearly,
    //     Contracts.Enums.PeriodType.Quarterly => PeriodType.Quarterly,
    //     _ => throw new ArgumentOutOfRangeException(nameof(dto), $"Unsupported PeriodType: {dto}")
    // };

}
