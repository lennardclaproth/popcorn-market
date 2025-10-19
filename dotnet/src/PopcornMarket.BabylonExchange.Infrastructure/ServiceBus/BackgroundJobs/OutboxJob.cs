using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;
using PopcornMarket.BabylonExchange.Persistence.Context;

namespace PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.BackgroundJobs;
internal sealed class OutboxJob : BackgroundService
{
    private readonly IProducer _producer;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxJob(IServiceScopeFactory scopeFactory, IProducer producer)
    {
        _producer = producer;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BabylonExchangeDbContext>();

            var messages = await db.OutboxMessages
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                var type = Type.GetType(message.Type)!;
                var @event = JsonSerializer.Deserialize(message.Payload, type);

                await _producer.Produce("tests", @event!, stoppingToken);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
