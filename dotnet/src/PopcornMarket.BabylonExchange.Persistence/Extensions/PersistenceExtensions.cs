using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Persistence.Context;
using PopcornMarket.BabylonExchange.Persistence.Repositories;

namespace PopcornMarket.BabylonExchange.Persistence.Extensions;

public static class PersistenceExtensions 
{
    public static IServiceCollection InstallPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BabylonExchangeDbContext>(options =>
                options.UseNpgsql(connectionString));

        SetupRepositories(services);
        
        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IOrderBookRepository, OrderBookRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
