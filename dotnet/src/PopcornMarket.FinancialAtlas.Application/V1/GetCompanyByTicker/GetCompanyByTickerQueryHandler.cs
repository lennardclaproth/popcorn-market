using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Errors;
using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetCompanyByTicker;

internal sealed class GetCompanyByTickerQueryHandler : IQueryHandler<GetCompanyByTickerQuery, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetCompanyByTickerQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyByTickerQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);

        if (company == null) return Result<CompanyDto>.Failure(CompanyErrors.CompanyNotFound);

        return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(company));
    }
}
