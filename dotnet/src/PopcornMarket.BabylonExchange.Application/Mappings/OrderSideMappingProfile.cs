using AutoMapper;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Application.Mappings;

public class OrderSideMappingProfile : Profile
{
    public OrderSideMappingProfile()
    {
        CreateMap<Contracts.Enums.OrderSide, OrderSide>();
        CreateMap<OrderSide, Contracts.Enums.OrderSide>();
    }
}
