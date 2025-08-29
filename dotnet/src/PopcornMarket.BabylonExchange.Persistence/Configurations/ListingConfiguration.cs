using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Constants;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable(name: nameof(Listing), schema: SchemaConstants.Schema);

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnType("UUID");
        
        builder.Property(l  => l.Ticker)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(l => l.OrderBook)
            .WithOne(o => o.Listing); 
        
        builder.Property(l => l.Name)
            .HasMaxLength(255);

        builder.Property(l => l.Isin)
            .IsRequired()
            .HasMaxLength(12);
    }
}
