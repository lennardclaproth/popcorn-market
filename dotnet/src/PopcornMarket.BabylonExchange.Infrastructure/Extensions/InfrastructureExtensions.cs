using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Infrastructure.Caching;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Consumers;
using PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Abstractions;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.BackgroundJobs;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Producers;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Services;
using PopcornMarket.SharedKernel.Abstractions;
using PopcornMarket.SharedKernel.Messaging;
using Elastic.Apm.Extensions;
using StackExchange.Redis;

namespace PopcornMarket.BabylonExchange.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConn = configuration.GetSection("Caching:Redis:ConnectionString").Value;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn!));
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        SetupOrderMatchingEngine(services);
        SetupKafkaMessaging(services);
        AddObservability(services, configuration);

        services.AddAllElasticApm();


        return services;
    }

    private static void SetupOrderMatchingEngine(IServiceCollection services)
    {
        services.AddSingleton(
            Channel.CreateBounded<Domain.Entities.Order>(new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            })
        );

        services.AddSingleton(
            Channel.CreateBounded<IDomainEvent>(new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            })
        );

        services.AddSingleton<OrderBookCache>(sp =>
        {
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var evictionTimeout = TimeSpan.FromMinutes(10);
            var logger = sp.GetRequiredService<ILogger<OrderBookCache>>();
            return new OrderBookCache(scopeFactory, evictionTimeout, logger);
        });
        services.AddSingleton<IOrderQueue>(sp =>
        {
            var channel = sp.GetRequiredService<Channel<Domain.Entities.Order>>();
            return new InMemoryOrderQueue(channel);
        });
        services.AddSingleton<IDomainEventQueue>(sp =>
        {
            var channel = sp.GetRequiredService<Channel<IDomainEvent>>();
            return new MatchingEngineEventQueue(channel);
        });
        services.AddHostedService<CacheEvictionService>();
        services.AddHostedService<MatchingEngine>();
        services.AddHostedService<MatchingEngineEventDispatcher>();
    }
    
    private static void SetupKafkaMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IConsumer, KafkaConsumer>();
        services.AddSingleton<IProducer, KafkaProducer>();
        services.AddHostedService<ConsumerJob>();
        services.AddHostedService<OutboxJob>();

        var handlers = InfrastructureAssemblyReference.Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => 
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(i => new { Type = i, HandlerType = type }))
            .ToList();

        // Should maybe become part of setup ServiceBus
        services.AddScoped<IOutboxService, EfCoreOutboxService>();

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Type, handler.HandlerType);
        }
    }

    private static void AddObservability(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAllElasticApm();
    }
}
