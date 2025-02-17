using Microsoft.EntityFrameworkCore;
using PopcornMarket.OrderBook.Domain.Entities;

namespace PopcornMarket.OrderBook.Persistence.Context;

public class OrderBookDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Domain.Entities.OrderBook> OrderBooks { get; set; } = null!;
    
    public OrderBookDbContext(DbContextOptions<OrderBookDbContext> options)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderBookDbContext).Assembly);
    }
}
