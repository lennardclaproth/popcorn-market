using AutoMapper;
using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetArticlesByTicker;

internal sealed class GetArticlesByTickerQueryHandler : IQueryHandler<GetArticlesByTickerQuery, IEnumerable<ArticleDto>>
{
    private readonly ICompanyArticleRepository _companyArticleRepository;
    private readonly IMapper _mapper;

    public GetArticlesByTickerQueryHandler(ICompanyArticleRepository companyArticleRepository, IMapper mapper)
    {
        _companyArticleRepository = companyArticleRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetArticlesByTickerQuery request, CancellationToken cancellationToken)
    {
        var articles = await _companyArticleRepository.FindByTicker(request.Ticker, request.Limit);
        
        var result = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Result<IEnumerable<ArticleDto>>.Success(result);
    }
}
