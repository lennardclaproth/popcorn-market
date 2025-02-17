using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.OrderBook.Persistence.Context;

namespace PopcornMarket.OrderBook.Persistence.Extensions;

public static class PersistenceExtensions 
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderBookDbContext>(options =>
                options.UseNpgsql(connectionString));

        return services;
    }
}
