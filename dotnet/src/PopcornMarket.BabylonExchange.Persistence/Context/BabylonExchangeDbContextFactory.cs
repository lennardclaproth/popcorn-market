using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PopcornMarket.BabylonExchange.Persistence.Context;

public class BabylonExchangeDbContextFactory : IDesignTimeDbContextFactory<BabylonExchangeDbContext>
{
    public BabylonExchangeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BabylonExchangeDbContext>();

        var connectionString = configuration.GetSection("Persistence:ConnectionString").Value;

        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlite("DataSource=migrations.db"); // Use SQLite if no PostgreSQL
        }

        return new BabylonExchangeDbContext(optionsBuilder.Options);
    }
}
