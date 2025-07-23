using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class BalanceSheetMappingProfile : Profile
{
    public BalanceSheetMappingProfile()
    {
        CreateMap<BalanceSheetDto, BalanceSheet>();
        CreateMap<BalanceSheet, BalanceSheetDto>();
    }
}
