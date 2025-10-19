using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Producers;
internal sealed class KafkaProducer : IProducer
{
    public Task Produce(string topic, object @event, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
