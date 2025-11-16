using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.UseCases.ListCompany;
internal sealed class ListCompanyCommandHandler : ICommandHandler<ListCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;

    public ListCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result> Handle(ListCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);
        Guard.Against.Null(company);
        company.List();
        await _companyRepository.Update(company.Id, company);
        return Result.Success();
    }
}
