using System.Collections.Immutable;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PopcornMarket.FinancialAtlas.Application.Abstractions;
using PopcornMarket.FinancialAtlas.Infrastructure.Messaging.Producers;

namespace PopcornMarket.FinancialAtlas.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProducer, KafkaProducer>();
        services.AddObservability(configuration);
    }

    private static void AddObservability(this IServiceCollection services, IConfiguration configuration)
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
}
