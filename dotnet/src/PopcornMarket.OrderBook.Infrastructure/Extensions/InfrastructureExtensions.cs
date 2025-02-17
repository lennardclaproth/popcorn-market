using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.OrderBook.Infrastructure.Caching;
using StackExchange.Redis;

namespace PopcornMarket.OrderBook.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string redisConn)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}
