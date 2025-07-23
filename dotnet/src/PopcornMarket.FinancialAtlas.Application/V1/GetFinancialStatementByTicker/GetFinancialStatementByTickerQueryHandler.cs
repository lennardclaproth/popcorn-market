using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Errors;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetFinancialStatementByTicker;

internal sealed class GetFinancialStatementByTickerQueryHandler : IQueryHandler<GetFinancialStatementByTickerQuery, FinancialStatementDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IFinancialStatementRepository _financialStatementRepository;
    private readonly IMapper _mapper;

    public GetFinancialStatementByTickerQueryHandler(ICompanyRepository companyRepository, IFinancialStatementRepository financialStatementRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _financialStatementRepository = financialStatementRepository;
        _mapper = mapper;
    }

    public async Task<Result<FinancialStatementDto>> Handle(GetFinancialStatementByTickerQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);
        if (company == null)
        {
            return Result<FinancialStatementDto>.Failure(CompanyErrors.CompanyNotFound);
        }
        
        var financialStatement = await _financialStatementRepository.GetMostRecent(request.Ticker, cancellationToken);

        var result = _mapper.Map<FinancialStatementDto>(financialStatement);

        return Result<FinancialStatementDto>.Success(result);
    }
}
