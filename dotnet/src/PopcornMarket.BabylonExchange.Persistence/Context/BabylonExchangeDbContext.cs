using Microsoft.EntityFrameworkCore;
using PopcornMarket.BabylonExchange.Domain.Entities;

namespace PopcornMarket.BabylonExchange.Persistence.Context;

public class BabylonExchangeDbContext : DbContext
{
    public DbSet<Order> BuyOrders { get; set; } = null!;
    public DbSet<Order> SellOrders { get; set; } = null!;
    public DbSet<Listing> Companies { get; set; } = null!;
    public DbSet<BabylonExchange.Domain.Entities.OrderBook> OrderBooks { get; set; } = null!;
    
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
    }
}
