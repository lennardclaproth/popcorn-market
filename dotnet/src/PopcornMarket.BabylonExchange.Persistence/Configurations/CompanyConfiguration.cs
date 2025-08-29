using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Persistence.Constants;

namespace PopcornMarket.BabylonExchange.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable(name: nameof(Company), schema: SchemaConstants.Schema);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnType("UUID");

        builder.Property(c  => c.Ticker)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.Name)
            .HasMaxLength(255);
        
        builder.Property(c => c.StockPriceUSD)
            .HasPrecision(18, 6)
            .HasColumnType("NUMERIC(18,6)")
            .IsRequired();
    }
}
