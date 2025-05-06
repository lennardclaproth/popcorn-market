using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PopcornMarket.FinancialTimes.Application.Abstractions;
using PopcornMarket.FinancialTimes.Infrastructure.Messaging.Consumers;
using PopcornMarket.FinancialTimes.Infrastructure.Messaging.Services;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.FinancialTimes.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        SetupKafkaMessaging(services);
        AddObservability(services, configuration);
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
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(configuration.GetSection("ServiceName").Value ?? "UnknownService"))
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
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
