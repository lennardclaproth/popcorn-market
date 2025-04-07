using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesBySector;

internal sealed class GetArticlesBySectorQueryHandler : IQueryHandler<GetArticlesBySectorQuery, IEnumerable<ArticleDto>>
{
    private readonly ISectorArticleRepository _sectorArticleRepository;
    private readonly IMapper _mapper;

    public GetArticlesBySectorQueryHandler(ISectorArticleRepository sectorArticleRepository, IMapper mapper)
    {
        _sectorArticleRepository = sectorArticleRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetArticlesBySectorQuery request, CancellationToken cancellationToken)
    {
        var articles = await _sectorArticleRepository.FindBySector(request.Sector, request.Limit);

        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
