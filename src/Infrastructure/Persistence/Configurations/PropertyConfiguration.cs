using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(p => p.NoReplyEmailAddress)
            .HasMaxLength(255);

        // Configure PropertyAddress value object
        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.StreetNumber)
                .HasColumnName("AddressStreetNumber")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.StreetName)
                .HasColumnName("AddressStreetName")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Configure PersonContactInfo value object for Superintendent
        builder.OwnsOne(p => p.Superintendent, superintendent =>
        {
            superintendent.Property(s => s.FirstName)
                .HasColumnName("SuperintendentFirstName")
                .HasMaxLength(100)
                .IsRequired();

            superintendent.Property(s => s.LastName)
                .HasColumnName("SuperintendentLastName")
                .HasMaxLength(100)
                .IsRequired();

            superintendent.Property(s => s.EmailAddress)
                .HasColumnName("SuperintendentEmailAddress")
                .HasMaxLength(255)
                .IsRequired();

            superintendent.Property(s => s.MobilePhone)
                .HasColumnName("SuperintendentMobilePhone")
                .HasMaxLength(20);
        });

        // Configure Units as JSON column for .NET 8
        builder.Property(p => p.Units)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("Units")
            .HasMaxLength(1000);

        // Configure relationships
        builder.HasMany(p => p.Tenants)
            .WithOne(t => t.Property)
            .HasForeignKey("PropertyId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure domain events to be ignored
        builder.Ignore(p => p.DomainEvents);
    }
}