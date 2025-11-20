using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.FinancialAtlas.Application.Abstractions;
using PopcornMarket.FinancialAtlas.Infrastructure.ServiceBus.Services;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Extensions;
namespace PopcornMarket.FinancialAtlas.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        List<string> topics = [TopicConstants.CompanyListed];
        services.WithKafkaServiceBus(configuration, topics, PayloadMap.Map, InfrastructureAssemblyReference.Assembly);
        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
        AddObservability(services);
    }

    private static void AddObservability(IServiceCollection services)
    {
        services.AddAllElasticApm();
    }
}
