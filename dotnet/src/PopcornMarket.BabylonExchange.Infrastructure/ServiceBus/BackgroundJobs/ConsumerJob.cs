using Microsoft.Extensions.Hosting;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.BackgroundJobs;

internal sealed class ConsumerJob : BackgroundService
{
    private readonly IConsumer _consumer;

    public ConsumerJob(IConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming(stoppingToken);
        return Task.CompletedTask;
    }
}
