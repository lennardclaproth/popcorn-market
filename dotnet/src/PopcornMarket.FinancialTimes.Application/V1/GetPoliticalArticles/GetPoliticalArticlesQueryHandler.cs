using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticles;

internal sealed class GetPoliticalArticlesQueryHandler : IQueryHandler<GetPoliticalArticlesQuery, IEnumerable<ArticleDto>>
{
    private readonly IPoliticalArticleRepository _politicalArticleRepository;
    private readonly IMapper _mapper;
    
    public GetPoliticalArticlesQueryHandler(IPoliticalArticleRepository politicalArticleRepository, IMapper mapper)
    {
        _politicalArticleRepository = politicalArticleRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetPoliticalArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _politicalArticleRepository.GetLimit(request.Limit);
        
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
