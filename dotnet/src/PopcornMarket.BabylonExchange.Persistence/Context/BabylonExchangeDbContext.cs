using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.ServiceBus.Helpers;
using PopcornMarket.ServiceBus.Models;

namespace PopcornMarket.BabylonExchange.Persistence.Context;

public class BabylonExchangeDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Listing> Listings { get; set; } = null!;
    public DbSet<OrderBook> OrderBooks { get; set; } = null!;
    public DbSet<Trade> Trades { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    public BabylonExchangeDbContext(DbContextOptions<BabylonExchangeDbContext> options)
        : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=:memory:"); // Use a temporary database for migrations
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(PersistenceAssemblyReference.Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageEfCoreConfiguration());
    }
}
