namespace PopcornMarket.FinancialTimes.Application.Abstractions;

public interface IConsumer
{
    Task StartConsuming(CancellationToken cancellationToken);
}
