using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticlesByRegion;

internal sealed class GetPoliticalArticlesByRegionQueryHandler : IQueryHandler<GetPoliticalArticlesByRegionQuery, IEnumerable<ArticleDto>>
{
    private readonly IPoliticalArticleRepository _politicalArticleRepository;
    private readonly IMapper _mapper;

    public GetPoliticalArticlesByRegionQueryHandler(IMapper mapper, IPoliticalArticleRepository politicalArticleRepository)
    {
        _mapper = mapper;
        _politicalArticleRepository = politicalArticleRepository;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetPoliticalArticlesByRegionQuery request, CancellationToken cancellationToken)
    {
        var articles = await _politicalArticleRepository.GetPoliticalArticlesByRegion(request.Region, request.Limit); 
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
