using Ardalis.GuardClauses;
using AutoMapper;
using Popcorn.FinancialAtlas.Domain.Abstractions;
using Popcorn.FinancialAtlas.Domain.Entities;
using Popcorn.FinancialAtlas.Domain.Errors;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.CreateCompany;

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IProducer _producer;
    private readonly IMapper _mapper;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IProducer producer, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _producer = producer;
        _mapper = mapper;
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
            request.Employees,
            request.Region);

        if (creationResult.IsFailure)
        {
            return creationResult;
        }
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));
        
        await _companyRepository.Add(creationResult.Value);
        #warning Lennard Claproth [13/11/2025] We violate clean architecture principles here at the moment for the sake of simplicity. This couples the event layer with the application layer.
        var companyCreatedPayload = _mapper.Map<CompanyCreatedPayload>(creationResult.Value);
        var companyCreatedIntegrationEvent = new CompanyCreatedIntegrationEvent(companyCreatedPayload);
        await _producer.Produce(companyCreatedIntegrationEvent.Topic, companyCreatedIntegrationEvent.Payload, cancellationToken);
        
        return Result.Success();
    }
}
