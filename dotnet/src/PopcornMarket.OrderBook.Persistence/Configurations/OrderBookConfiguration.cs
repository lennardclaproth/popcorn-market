using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.OrderBook.Persistence.Constants;

namespace PopcornMarket.OrderBook.Persistence.Configurations;

internal sealed class OrderBookConfiguration : IEntityTypeConfiguration<Domain.Entities.OrderBook>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.OrderBook> builder)
    {
        builder.ToTable(name: nameof(Domain.Entities.OrderBook), schema: SchemaConstants.Schema);

        builder.HasKey(ob => ob.Id);

        builder.Property(ob => ob.Id)
            .HasColumnType("UUID");

        builder.Property(ob => ob.Ticker)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.HasMany(ob => ob.BuyOrders)
            .WithOne(o => o.OrderBook)
            .HasForeignKey(o => o.OrderBookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(ob => ob.SellOrders)
            .WithOne(o => o.OrderBook)
            .HasForeignKey(o => o.OrderBookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(ob => ob.Ticker);
    }
}
