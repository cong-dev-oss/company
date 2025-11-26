using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryService.Domain.Entities;

namespace InventoryService.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.ProductId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Index for faster lookups
        builder.HasIndex(s => s.ProductId);
    }
}


