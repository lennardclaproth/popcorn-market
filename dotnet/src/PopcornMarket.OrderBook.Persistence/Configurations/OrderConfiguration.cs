using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.OrderBook.Domain.Entities;
using PopcornMarket.OrderBook.Persistence.Constants;

namespace PopcornMarket.OrderBook.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(name: nameof(Order), schema: SchemaConstants.Schema);
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasColumnType("UUID")
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(o => o.StockSymbol)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.Property(o => o.TraderId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(o => o.Price)
            .HasPrecision(18, 2)
            .HasColumnType("NUMERIC(18,2)")
            .IsRequired();
            
        builder.Property(o => o.Quantity)
            .IsRequired();
        
        builder.HasDiscriminator<string>("OrderType")
            .HasValue<BuyOrder>(nameof(BuyOrder))
            .HasValue<SellOrder>(nameof(SellOrder));
        
        builder.Property(o => o.Timestamp)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        
        builder.Property(o => o.Status)
            .IsRequired();
        
        builder.HasIndex(o => o.Timestamp);
    }
}
