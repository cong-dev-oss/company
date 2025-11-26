using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        // Configure value object as owned entity
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").IsRequired();
            address.Property(a => a.City).HasColumnName("ShippingCity").IsRequired();
            address.Property(a => a.State).HasColumnName("ShippingState");
            address.Property(a => a.ZipCode).HasColumnName("ShippingZipCode");
            address.Property(a => a.Country).HasColumnName("ShippingCountry").IsRequired();
        });

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired();

        // Configure collection
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}







