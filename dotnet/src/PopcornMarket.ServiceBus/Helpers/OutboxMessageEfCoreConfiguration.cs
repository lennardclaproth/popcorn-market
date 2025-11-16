using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.ServiceBus.Models;

namespace PopcornMarket.ServiceBus.Helpers;
public class OutboxMessageEfCoreConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.Property(o => o.Id)
            .HasColumnType("UUID")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnType("timestamptz");

        builder.Property(x => x.ProcessedAtUtc)
            .HasColumnType("timestamptz");

        builder.Property(x => x.Topic)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasKey(x => x.Id);
    }
}
