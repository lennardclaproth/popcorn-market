namespace PopcornMarket.ServiceBus.Abstractions
{
    public interface IProducer
    {
        public Task Produce<T>(string topic, T payload, CancellationToken cancellationToken = default);
    }
}
