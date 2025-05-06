using Ardalis.GuardClauses;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Domain.Entities;
using PopcornMarket.OrderBook.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.OrderBook.Application.V1.CreateCompany;

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand>
{
    
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    /// <summary>
    /// Checks if a company already exists, if the company does not exist
    /// it creates a new company with an initial stock price.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);
        if(company != null) return Result.Failure(CompanyErrors.CompanyAlreadyExists);
        
        var creationResult = Company.Create(request.Ticker, request.Name, request.StockPriceUSD);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value), "The creation result cannot be null.");
        
        await _companyRepository.AddEntity(creationResult.Value);
        
        return Result.Success();
    }
}
