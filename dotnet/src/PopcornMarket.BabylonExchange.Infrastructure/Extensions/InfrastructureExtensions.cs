using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Infrastructure.Caching;
using PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;
using PopcornMarket.BabylonExchange.Infrastructure.ServiceBus.Services;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Extensions;
using PopcornMarket.SharedKernel.Abstractions;
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
        SetupKafkaMessaging(services, configuration);
        AddObservability(services);

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
    
    private static void SetupKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        List<string> topics = new();
        services.WithKafkaServiceBus(configuration, topics, PayloadMap.Map, InfrastructureAssemblyReference.Assembly)
            .WithEfCoreOutbox<Persistence.Context.BabylonExchangeDbContext>();

        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
    }

    private static void AddObservability(IServiceCollection services)
    {
        services.AddAllElasticApm();
    }
}
