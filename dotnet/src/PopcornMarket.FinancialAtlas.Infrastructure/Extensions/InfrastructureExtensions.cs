using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.FinancialAtlas.Application.Abstractions;
using PopcornMarket.FinancialAtlas.Infrastructure.Messaging.Producers;

namespace PopcornMarket.FinancialAtlas.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IProducer, KafkaProducer>();
    }
}
