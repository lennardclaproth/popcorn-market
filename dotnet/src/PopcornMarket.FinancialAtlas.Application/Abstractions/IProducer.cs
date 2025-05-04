namespace PopcornMarket.FinancialAtlas.Application.Abstractions;

public interface IProducer
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
}
