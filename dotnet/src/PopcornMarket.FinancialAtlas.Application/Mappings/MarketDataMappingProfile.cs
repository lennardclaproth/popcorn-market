using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class MarketDataMappingProfile : Profile
{
    public MarketDataMappingProfile()
    {
        CreateMap<MarketDataDto, MarketData>();
        CreateMap<MarketData, MarketDataDto>();
    }
}
