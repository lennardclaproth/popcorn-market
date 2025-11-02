using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class MatchingEngineEventDispatcher : BackgroundService
{
    private readonly ILogger<MatchingEngineEventDispatcher> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Channel<IDomainEvent> _channel;

    public MatchingEngineEventDispatcher(ILogger<MatchingEngineEventDispatcher> logger, Channel<IDomainEvent> channel, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _channel = channel;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var domainEvent in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessEventSafely(domainEvent, stoppingToken);
        }
    }

    private async Task ProcessEventSafely(IDomainEvent domainEvent, CancellationToken stoppingToken)
    {
        const int maxRetries = 3;
        var currentAttempt = 0;
        _logger.LogInformation("Current amount of events in queue: {EventCount}", _channel.Reader.Count);
        
        while (currentAttempt < maxRetries)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                //var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                _logger.LogInformation("Handling domain event {EventType} (attempt {Attempt}/{MaxRetries})",
                    domainEvent.GetType().Name, currentAttempt + 1, maxRetries);
                await mediator.Publish(domainEvent, stoppingToken);

                _logger.LogDebug("Successfully processed domain event {EventType}", domainEvent.GetType().Name);
                return;
            }
            catch (Exception ex) when (currentAttempt < maxRetries - 1)
            {
                currentAttempt++;
                var delay = TimeSpan.FromSeconds(Math.Pow(2, currentAttempt)); // Exponential backoff
                
                _logger.LogWarning(ex, "Error dispatching domain event {EventType} (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s", 
                    domainEvent.GetType().Name, currentAttempt, maxRetries, delay.TotalSeconds);
                    
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch domain event {EventType} after {MaxRetries} attempts. Event will be discarded to prevent memory leak.", 
                    domainEvent.GetType().Name, maxRetries);
                return;
            }
        }
    }
}
