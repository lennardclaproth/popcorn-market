using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Entities;

namespace PopcornMarket.FinancialTimes.Application.Mappings;

public sealed class ArticleMappingProfile : Profile
{
    public ArticleMappingProfile()
    {
        CreateMap<Article, ArticleDto>();

        CreateMap<CompanyArticle, ArticleDto>()
            .ForMember(dest => dest.Ticker, opt => opt.MapFrom(src => src.Ticker))
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Sector))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName));

        CreateMap<MacroEconomicArticle, ArticleDto>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region));

        CreateMap<PoliticalArticle, ArticleDto>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region));

        CreateMap<SectorArticle, ArticleDto>()
            .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.Sector));
    }
}
