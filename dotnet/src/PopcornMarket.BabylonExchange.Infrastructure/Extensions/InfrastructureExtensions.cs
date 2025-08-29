using System.Threading.Channels;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Infrastructure.Caching;
using PopcornMarket.BabylonExchange.Infrastructure.Messaging.Consumers;
using PopcornMarket.BabylonExchange.Infrastructure.Messaging.Services;
using PopcornMarket.BabylonExchange.Infrastructure.OrderExecutionEngine;
using PopcornMarket.SharedKernel.Messaging;
using StackExchange.Redis;

namespace PopcornMarket.BabylonExchange.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConn = configuration.GetSection("Caching:Redis:ConnectionString").Value;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn!));
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        SetupOrderExecutionEngine(services);
        SetupKafkaMessaging(services);
        AddObservability(services, configuration);
        
        return services;
    }

    private static void SetupOrderExecutionEngine(IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<Domain.Entities.Order>());
        services.AddSingleton<IOrderQueue, InMemoryOrderQueue>();
        services.AddHostedService<MatchingEngine>();
    }
    
    private static void SetupKafkaMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IConsumer, KafkaConsumer>();
        services.AddHostedService<ConsumerService>();
        
        var handlers = InfrastructureAssemblyReference.Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => 
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(i => new { Type = i, HandlerType = type }))
            .ToList();

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Type, handler.HandlerType);
        }
    }

    private static void AddObservability(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConfluentKafkaInstrumentation()
                    .AddRedisInstrumentation()
                    .AddNpgsql()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(configuration.GetSection("ServiceName").Value ?? "UnknownService"))
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
    }
}
