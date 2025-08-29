using MediatR;
using Microsoft.Extensions.Hosting;

namespace PopcornMarket.BabylonExchange.Infrastructure.BackgroundJobs;

internal sealed class ExecuteBuyOrderJob : BackgroundService
{
    private readonly ISender _sender;

    public ExecuteBuyOrderJob(ISender sender)
    {
        _sender = sender;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        throw new NotImplementedException();
    }
}
