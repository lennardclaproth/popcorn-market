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

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnType("UUID");

        builder.Property(c  => c.Ticker)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.Name)
            .HasMaxLength(255);

        builder.Property(c => c.Isin)
            .IsRequired()
            .HasMaxLength(12);
    }
}
