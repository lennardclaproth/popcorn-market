namespace PopcornMarket.FinancialTimes.Application.Abstractions;

public interface IConsumer
{
    public Task StartConsuming(CancellationToken cancellationToken);
}
