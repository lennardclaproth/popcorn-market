using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        
    }
}
