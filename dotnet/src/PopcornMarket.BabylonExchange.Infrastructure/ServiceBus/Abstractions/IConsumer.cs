namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;

internal interface IConsumer
{
    public Task StartConsuming(CancellationToken cancellationToken);
}
