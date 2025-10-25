using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Producers;
internal sealed class KafkaProducer : IProducer
{
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(ILogger<KafkaProducer> logger)
    {
        _logger = logger;
    }

    public Task Produce(string topic, object @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Producing event to topic {Topic}: {@Event}", topic, @event);
        return Task.CompletedTask;
    }
}
