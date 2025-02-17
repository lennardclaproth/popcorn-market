using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PopcornMarket.OrderBook.Persistence.Context;

public class OrderBookDbContextFactory : IDesignTimeDbContextFactory<OrderBookDbContext>
{
    public OrderBookDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrderBookDbContext>();

        var connectionString = configuration.GetConnectionString("OrderBookDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlite("DataSource=migrations.db"); // Use SQLite if no PostgreSQL
        }

        return new OrderBookDbContext(optionsBuilder.Options);
    }
}
