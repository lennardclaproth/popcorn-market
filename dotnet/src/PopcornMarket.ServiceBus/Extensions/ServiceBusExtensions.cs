using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.ServiceBus.BackgroundJobs;
using PopcornMarket.ServiceBus.Consumers;
using PopcornMarket.ServiceBus.Producers;
using PopcornMarket.ServiceBus.Services;

namespace PopcornMarket.ServiceBus.Extensions;
public static class ServiceBusExtensions
{
    public static IServiceCollection WithKafkaServiceBus(this IServiceCollection services, IConfiguration configuration,
        List<string> topics, Dictionary<string, Type> payloadMap, Assembly assembly)

    {
        services.AddSingleton(provider =>
        {
            var svc = new TopicService(configuration);
            svc.AddTopics(topics);
            svc.AddPayloadMap(payloadMap);
            return svc;
        });

        services.AddSingleton<IConsumer, KafkaConsumer>();
        services.AddSingleton<IProducer, KafkaProducer>();
        services.AddHostedService<ConsumerJob>();

        var handlers = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type =>
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                    .Select(i => new { Type = i, HandlerType = type }))
            .ToList();

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.Type, handler.HandlerType);
        }

        return services;
    }

    public static IServiceCollection WithEfCoreOutbox<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IOutbox, EfCoreOutboxService<TDbContext>>();
        services.AddHostedService<OutboxJob>();
        return services;
    }

    public static IServiceCollection WithMongoOutbox(
        this IServiceCollection services,
        string collectionName = "outbox_messages")
    {
        services.AddSingleton<IOutbox>(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return new MongoOutboxService(db, collectionName);
        });

        services.AddHostedService<OutboxJob>();

        return services;
    }
}
