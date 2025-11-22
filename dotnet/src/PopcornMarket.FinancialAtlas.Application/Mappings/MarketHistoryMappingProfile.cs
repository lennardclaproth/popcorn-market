using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class MarketHistoryMappingProfile : Profile
{
    public MarketHistoryMappingProfile()
    {
        // DTO -> Domain mappings
        CreateMap<MarketHistoryDto, MarketHistory>();
            
        // If needed: Domain -> DTO
        CreateMap<MarketHistory, MarketHistoryDto>();
    }
}
