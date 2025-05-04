using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.Messaging.Contracts.V1.Events;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Company, CompanyCreatedEvent>();
    }
}
