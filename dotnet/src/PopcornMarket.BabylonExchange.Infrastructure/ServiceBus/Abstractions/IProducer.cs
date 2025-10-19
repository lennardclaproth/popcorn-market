namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;
internal interface IProducer
{
    public Task Produce(string topic, object @event, CancellationToken cancellationToken = default);
}
