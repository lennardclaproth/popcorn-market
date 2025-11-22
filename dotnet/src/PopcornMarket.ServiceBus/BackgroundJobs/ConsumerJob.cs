using Microsoft.Extensions.Hosting;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.ServiceBus.BackgroundJobs;
internal sealed class ConsumerJob : BackgroundService
{
    private readonly IConsumer _consumer;

    public ConsumerJob(IConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _consumer.StartConsuming(stoppingToken);
    }
}

