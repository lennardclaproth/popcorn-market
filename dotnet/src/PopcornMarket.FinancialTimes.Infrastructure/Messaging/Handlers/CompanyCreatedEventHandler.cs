using MediatR;
using Microsoft.Extensions.Logging;
using PopcornMarket.FinancialTimes.Application.V1.CreateCompany;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.FinancialTimes.Infrastructure.Messaging.Handlers;

internal sealed class CompanyCreatedEventHandler : IEventHandler<CompanyCreatedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<CompanyCreatedEventHandler> _logger;

    public CompanyCreatedEventHandler(ISender mediator, ILogger<CompanyCreatedEventHandler> logger)
    {
        _sender = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Handles the company created event, is responsible for everything regarding
    /// the event itself and then completing the action/ use case by calling it's
    /// command handler.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task HandleAsync(CompanyCreatedEvent @event, CancellationToken cancellationToken)
    {
        var createCompanyCommand = new CreateCompanyCommand
        {
            Ticker = @event.Ticker,
            Name = @event.Name,
            Industry = @event.Industry,
            Description = @event.Description,
            Headquarters = @event.Headquarters,
            Ceo = @event.Ceo,
            FoundedYear = @event.FoundedYear,
            Employees = @event.Employees,
            Region = @event.Region
        };
        
        var result = await _sender.Send(createCompanyCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "Failed to create company from event. ErrorType: {ErrorType}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                result.Error.Type,
                result.Error.Code,
                result.Error.Description);
        }
    }
}
