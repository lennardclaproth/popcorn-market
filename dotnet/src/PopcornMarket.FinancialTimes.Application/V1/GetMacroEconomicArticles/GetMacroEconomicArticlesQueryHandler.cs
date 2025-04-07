using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticles;

internal sealed class GetMacroEconomicArticlesQueryHandler : IQueryHandler<GetMacroEconomicArticlesQuery, IEnumerable<ArticleDto>>
{
    private readonly IMacroEconomicArticleRepository _macroEconomicArticleRepository;
    private readonly IMapper _mapper;

    public GetMacroEconomicArticlesQueryHandler(IMacroEconomicArticleRepository macroEconomicArticleRepository, IMapper mapper)
    {
        _macroEconomicArticleRepository = macroEconomicArticleRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetMacroEconomicArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _macroEconomicArticleRepository.GetLimit(request.Limit);
        
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
