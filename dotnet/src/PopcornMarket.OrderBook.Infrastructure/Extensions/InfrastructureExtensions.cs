using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PopcornMarket.OrderBook.Application.Abstractions;
using PopcornMarket.OrderBook.Infrastructure.Caching;
using PopcornMarket.OrderBook.Infrastructure.Messaging.Consumers;
using PopcornMarket.OrderBook.Infrastructure.Messaging.Services;
using PopcornMarket.SharedKernel.Messaging;
using StackExchange.Redis;

namespace PopcornMarket.OrderBook.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConn = configuration.GetSection("Caching:Redis:ConnectionString").Value;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn!));
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        SetupKafkaMessaging(services);
        AddObservability(services, configuration);
        
        return services;
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
