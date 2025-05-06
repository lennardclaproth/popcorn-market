using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.OrderBook.Domain.Entities;
using PopcornMarket.OrderBook.Persistence.Constants;

namespace PopcornMarket.OrderBook.Persistence.Configurations;

internal sealed class SellOrderConfiguration : IEntityTypeConfiguration<SellOrder>
{
    public void Configure(EntityTypeBuilder<SellOrder> builder)
    {
        builder.ToTable(name: nameof(SellOrder), schema: SchemaConstants.Schema);
        
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
        
        builder.Property(o => o.Timestamp)
            .IsRequired()
            .HasColumnType("TIMESTAMPTZ");
        
        builder.Property(o => o.Status)
            .IsRequired();
        
        builder.HasIndex(o => o.Timestamp);
    }
}
