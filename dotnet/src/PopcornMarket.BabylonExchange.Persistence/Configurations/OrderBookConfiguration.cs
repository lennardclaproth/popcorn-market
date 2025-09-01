using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Constants;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;

internal sealed class OrderBookConfiguration : IEntityTypeConfiguration<OrderBook>
{
    public void Configure(EntityTypeBuilder<OrderBook> builder)
    {
        builder.ToTable(name: nameof(OrderBook), schema: SchemaConstants.Schema);

        builder.HasKey(ob => ob.Id);

        builder.Property(ob => ob.Id)
            .HasColumnType("UUID");

        builder.Property(ob => ob.Ticker)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(ob => ob.CurrentPrice)
            .HasPrecision(18, 6)
            .HasColumnType("NUMERIC(18,6)");
        
        builder.HasMany(ob => ob.Orders)
            .WithOne(o => o.OrderBook)
            .HasForeignKey(o => o.OrderBookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ob => ob.Listing)
            .WithOne(l => l.OrderBook)
            .HasForeignKey<OrderBook>(ob => ob.ListingId);
        
        builder.HasIndex(ob => ob.Ticker);
    }
}
