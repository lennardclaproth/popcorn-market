using AutoMapper;
using PopcornMarket.BabylonExchange.Domain.Enums;

namespace PopcornMarket.BabylonExchange.Application.Mappings;

public class OrderTypeMappingProfile : Profile
{
    public OrderTypeMappingProfile()
    {
        CreateMap<Contracts.Enums.OrderType, OrderType>();
        CreateMap<OrderType, Contracts.Enums.OrderType>();
    }
}
