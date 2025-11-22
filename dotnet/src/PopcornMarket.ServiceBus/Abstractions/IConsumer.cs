namespace PopcornMarket.ServiceBus.Abstractions;
public interface IConsumer
{
    public Task StartConsuming(CancellationToken cancellationToken);
}
