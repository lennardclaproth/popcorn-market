using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticlesByRegion;

internal sealed class GetMacroEconomicArticlesByRegionQueryHandler : IQueryHandler<GetMacroEconomicArticlesByRegionQuery, IEnumerable<ArticleDto>>
{
    private readonly IMacroEconomicArticleRepository _macroEconomicArticleRepository;
    private readonly IMapper _mapper;

    public GetMacroEconomicArticlesByRegionQueryHandler(IMapper mapper, IMacroEconomicArticleRepository macroEconomicArticleRepository)
    {
        _mapper = mapper;
        _macroEconomicArticleRepository = macroEconomicArticleRepository;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetMacroEconomicArticlesByRegionQuery request, CancellationToken cancellationToken)
    {
        var articles = await _macroEconomicArticleRepository.GetArticlesByRegion(request.Region, request.Limit); 
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
