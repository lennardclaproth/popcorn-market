using MediatR;
using Microsoft.Extensions.Logging;
using PopcornMarket.FinancialTimes.Application.V1.UseCases.CreateCompany;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialTimes.Infrastructure.ServiceBus.Handlers;

internal sealed class CompanyCreatedIntegrationEventHandler : IntegrationEventHandler<CompanyCreatedPayload>
{
    private readonly ISender _sender;
    private readonly ILogger<CompanyCreatedIntegrationEventHandler> _logger;

    public CompanyCreatedIntegrationEventHandler(ISender mediator, ILogger<CompanyCreatedIntegrationEventHandler> logger)
    {
        _sender = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Handles the company created payload, is responsible for everything regarding
    /// the payload itself and then completing the action/ use case by calling it's
    /// command handler.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task Handle(CompanyCreatedPayload payload, CancellationToken cancellationToken)
    {
        var createCompanyCommand = new CreateCompanyCommand
        {
            Ticker = payload.Ticker,
            Name = payload.Name,
            Industry = payload.Industry,
            Description = payload.Description,
            Headquarters = payload.Headquarters,
            Ceo = payload.Ceo,
            FoundedYear = payload.FoundedYear,
            Employees = payload.Employees,
            Region = payload.Region
        };
        
        var result = await _sender.Send(createCompanyCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "Failed to create company from payload. ErrorType: {ErrorType}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                result.Error.Type,
                result.Error.Code,
                result.Error.Description);
        }
    }
}
