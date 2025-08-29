namespace PopcornMarket.BabylonExchange.Application.Abstractions;

public interface IConsumer
{
    public Task StartConsuming(CancellationToken cancellationToken);
}
