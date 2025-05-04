using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class MarketSnapshotMappingProfile : Profile
{
    public MarketSnapshotMappingProfile()
    {
        // DTO -> Domain mappings
        CreateMap<MarketSnapshotDto, MarketSnapshot>();

        // If needed: Domain -> DTO
        CreateMap<MarketSnapshot, MarketSnapshotDto>();
    }
}
