using AutoMapper;
using Popcorn.FinancialAtlas.Domain.ValueObjects;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;

namespace PopcornMarket.FinancialAtlas.Application.Mappings;

public sealed class CashFlowStatementMappingProfile : Profile
{
    public CashFlowStatementMappingProfile()
    {
        CreateMap<CashFlowStatement, CashFlowStatementDto>();
        CreateMap<CashFlowStatementDto, CashFlowStatement>();
    }
}
