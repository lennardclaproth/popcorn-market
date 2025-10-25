using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Persistence.Entities;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.Property(o => o.Id)
            .HasColumnType("UUID")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.OccurredOnUtc)
            .HasColumnType("timestamptz");

        builder.Property(x => x.ProcessedOnUtc)
            .HasColumnType("timestamptz");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasKey(x => x.Id);
    }
}

