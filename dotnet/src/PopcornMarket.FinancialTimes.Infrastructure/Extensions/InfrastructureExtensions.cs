using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.ServiceBus.Extensions;

namespace PopcornMarket.FinancialTimes.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void InstallInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        SetupKafkaMessaging(services, configuration);
        AddObservability(services);
    }

    private static void AddObservability(IServiceCollection services)
    {
        services.AddAllElasticApm();
    }

    private static void SetupKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddSingleton<IConsumer, KafkaConsumer>();
        //services.AddHostedService<ConsumerService>();
        List<string> topics = [TopicConstants.CompanyCreated, TopicConstants.CompanyListed];
        services.WithKafkaServiceBus(configuration, topics, PayloadMap.Map, InfrastructureAssemblyReference.Assembly);
    }
}
