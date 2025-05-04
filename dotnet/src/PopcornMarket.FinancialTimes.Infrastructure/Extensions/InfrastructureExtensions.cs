using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.FinancialTimes.Application.Abstractions;
using PopcornMarket.FinancialTimes.Infrastructure.Messaging.Consumers;
using PopcornMarket.FinancialTimes.Infrastructure.Messaging.Services;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.FinancialTimes.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services)
    {
        SetupKafkaMessaging(services);
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
}
