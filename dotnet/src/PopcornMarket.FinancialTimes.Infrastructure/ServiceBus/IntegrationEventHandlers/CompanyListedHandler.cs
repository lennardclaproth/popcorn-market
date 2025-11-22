using MediatR;
using PopcornMarket.FinancialTimes.Application.V1.UseCases.ListCompany;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialTimes.Infrastructure.ServiceBus.IntegrationEventHandlers;
internal sealed class CompanyListedHandler : IntegrationEventHandler<CompanyListedPayload>
{
    private readonly ISender _sender;

    public CompanyListedHandler(ISender sender)
    {
        _sender = sender;
    }

    public override async Task Handle(CompanyListedPayload payload, CancellationToken cancellationToken)
    {
        var command = new ListCompanyCommand
        {
            Ticker = payload.Ticker
        };

        await _sender.Send(command, cancellationToken);
    }
}
