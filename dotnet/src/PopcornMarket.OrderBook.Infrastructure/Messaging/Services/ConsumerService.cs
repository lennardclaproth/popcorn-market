using Microsoft.Extensions.Hosting;
using PopcornMarket.OrderBook.Application.Abstractions;

namespace PopcornMarket.OrderBook.Infrastructure.Messaging.Services;

internal sealed class ConsumerService : BackgroundService
{
    private readonly IConsumer _consumer;

    public ConsumerService(IConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming(stoppingToken);
        return Task.CompletedTask;
    }
}
