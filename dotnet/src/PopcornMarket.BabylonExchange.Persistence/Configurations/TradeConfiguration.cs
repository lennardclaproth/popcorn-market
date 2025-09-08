using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Constants;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.ToTable(nameof(Trade), SchemaConstants.Schema);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(t => t.Quantity)
            .HasColumnType("integer")
            .IsRequired();

        builder.HasOne(t => t.BuyOrder)
            .WithMany()
            .HasForeignKey(t => t.BuyOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.SellOrder)
            .WithMany()
            .HasForeignKey(t => t.SellOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
