using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.ServiceBus.Abstractions;

namespace PopcornMarket.ServiceBus.BackgroundJobs;
internal sealed class OutboxJob : BackgroundService
{
    private readonly ILogger<OutboxJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OutboxJob(ILogger<OutboxJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Small poll loop – tune interval & batch size as needed
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                var producer = scope.ServiceProvider.GetRequiredService<IProducer>();

                var batch = await outbox.GetUnprocessedBatchAsync(50, stoppingToken);

                if (batch.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                foreach (var message in batch)
                {
                    try
                    {
                        await producer.Produce(
                            message.Topic,
                            message.Payload,
                            stoppingToken);

                        await outbox.MarkProcessedAsync(message, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to dispatch outbox message {Id}", message.Id);
                        await outbox.MarkFailedAsync(message, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in outbox dispatcher loop");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
