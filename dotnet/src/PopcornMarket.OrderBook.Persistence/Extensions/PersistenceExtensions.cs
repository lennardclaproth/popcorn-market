using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Persistence.Context;
using PopcornMarket.OrderBook.Persistence.Repositories;

namespace PopcornMarket.OrderBook.Persistence.Extensions;

public static class PersistenceExtensions 
{
    public static IServiceCollection InstallPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderBookDbContext>(options =>
                options.UseNpgsql(connectionString));

        SetupRepositories(services);
        
        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IOrderBookRepository, OrderBookRepository>();
        services.AddScoped<IBuyOrderRepository, BuyOrderRepository>();
        services.AddScoped<ISellOrderRepository, SellOrderRepository>();
    }
}
