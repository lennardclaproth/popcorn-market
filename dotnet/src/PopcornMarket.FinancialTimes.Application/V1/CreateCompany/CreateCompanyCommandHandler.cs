using Ardalis.GuardClauses;
using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.FinancialTimes.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.CreateCompany;

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var existingCompany = await _companyRepository.GetByTicker(request.Ticker);

        if (existingCompany != null)
        {
            return Result.Failure(CompanyErrors.CompanyAlreadyExists);
        }

        var creationResult = Company.Create(
            request.Ticker,
            request.Name,
            request.Industry,
            request.Description,
            request.Headquarters,
            request.Ceo,
            request.FoundedYear,
            request.Employees);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));
        
        await _companyRepository.Add(creationResult.Value);
        
        return Result.Success();
    }
}

