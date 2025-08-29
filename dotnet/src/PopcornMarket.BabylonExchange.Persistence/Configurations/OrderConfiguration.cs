using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Constants;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
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
            .HasPrecision(18, 6)
            .HasColumnType("NUMERIC(18,6)")
            .IsRequired();
            
        builder.Property(o => o.Quantity)
            .IsRequired();
        
        builder.Property(o => o.PlacedTimestamp)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        
        builder.Property(o => o.ExecutedTimestamp)
            .HasColumnType("TIMESTAMPTZ");
        
        builder.Property(o => o.Status)
            .IsRequired();
        
        builder.HasIndex(o => o.PlacedTimestamp);
        
        builder.HasIndex(o => o.ExecutedTimestamp);
    }
}
