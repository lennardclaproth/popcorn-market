using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesByRegion;

internal sealed class GetArticlesByRegionQueryHandler : IQueryHandler<GetPoliticalArticlesByRegionQuery, IEnumerable<ArticleDto>>
{
    private readonly IPoliticalArticleRepository _sectorArticleRepository;
    private readonly IMapper _mapper;

    public GetArticlesByRegionQueryHandler(IMapper mapper, IPoliticalArticleRepository sectorArticleRepository)
    {
        _mapper = mapper;
        _sectorArticleRepository = sectorArticleRepository;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetPoliticalArticlesByRegionQuery request, CancellationToken cancellationToken)
    {
        var articles = await _sectorArticleRepository.GetPoliticalArticlesByRegion(request.Region, request.Limit); 
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
