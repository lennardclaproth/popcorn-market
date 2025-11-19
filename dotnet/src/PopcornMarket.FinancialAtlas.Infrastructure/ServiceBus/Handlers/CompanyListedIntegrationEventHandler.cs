using MediatR;
using PopcornMarket.FinancialAtlas.Application.V1.UseCases.ListCompany;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.FinancialAtlas.Infrastructure.ServiceBus.Handlers;
internal sealed class CompanyListedIntegrationEventHandler : IntegrationEventHandler<CompanyListedPayload>
{
    private readonly ISender _sender;

    public CompanyListedIntegrationEventHandler(ISender sender)
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
