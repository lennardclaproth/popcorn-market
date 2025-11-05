using System.Threading.Channels;
using Elastic.Apm;
using Elastic.Apm.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;

internal sealed class MatchingEngineEventDispatcher : BackgroundService
{
    private readonly ILogger<MatchingEngineEventDispatcher> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Channel<IDomainEvent> _channel;
    private readonly ITracer _tracer;

    public MatchingEngineEventDispatcher(ILogger<MatchingEngineEventDispatcher> logger, Channel<IDomainEvent> channel, IServiceScopeFactory scopeFactory, ITracer tracer)
    {
        _logger = logger;
        _channel = channel;
        _scopeFactory = scopeFactory;
        _tracer = tracer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var domainEvent in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessEvent(domainEvent, stoppingToken);
        }
    }

    private async Task ProcessEvent(IDomainEvent domainEvent, CancellationToken stoppingToken)
    {
        var transaction = _tracer.StartTransaction($"{nameof(MatchingEngineEventDispatcher)}.{nameof(ProcessEvent)}", nameof(BackgroundService));
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(domainEvent, stoppingToken);
            _logger.LogDebug("Successfully processed domain event {EventType}", domainEvent.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch domain event {EventType}. Event will be discarded to prevent memory leak.", domainEvent.GetType().Name);
            transaction.CaptureException(ex);
        }
        finally
        {
            transaction.End();
        }
    }
}
