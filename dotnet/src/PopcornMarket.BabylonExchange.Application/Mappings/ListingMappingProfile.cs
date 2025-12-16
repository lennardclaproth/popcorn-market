using AutoMapper;
using PopcornMarket.BabylonExchange.Contracts.Dtos;
using PopcornMarket.BabylonExchange.Domain.Entities;

internal sealed class ListingMappingProfile : Profile
{
    public ListingMappingProfile()
    {
        CreateMap<Listing, ListingDto>()
            .ForMember(dest => dest.StockSymbol, opt => opt.MapFrom(src => src.StockSymbol))
            .ForMember(dest => dest.Isin, opt => opt.MapFrom(src => src.Isin))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
# warning Lennard Claproth [10/12/2025] The last price is invalid here.
            .ForMember(dest => dest.LastPrice, opt => opt.MapFrom(src => src.ClosePrice))
            .ForMember(dest => dest.PriceChange, opt => opt.MapFrom(src => src.ClosePrice - src.OpenPrice))
            .ForMember(dest => dest.PriceChangePercent, opt => opt.MapFrom(src => 
                src.OpenPrice != 0 ? ((src.ClosePrice - src.OpenPrice) / src.OpenPrice) * 100 : 0))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdate));
    }
}
