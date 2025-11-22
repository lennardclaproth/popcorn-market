using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;
public sealed class AnalysisMappingProfile : Profile
{
    public AnalysisMappingProfile()
    {
        CreateMap<Analysis, AnalysisDto>();
        CreateMap<AnalysisDto, Analysis>();
    }
}
