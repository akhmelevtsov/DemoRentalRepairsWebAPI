using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UnitNumber)
            .IsRequired()
            .HasMaxLength(20);

        // Configure PersonContactInfo value object
        builder.OwnsOne(t => t.ContactInfo, contactInfo =>
        {
            contactInfo.Property(c => c.FirstName)
                .HasColumnName("ContactInfoFirstName")
                .HasMaxLength(100)
                .IsRequired();

            contactInfo.Property(c => c.LastName)
                .HasColumnName("ContactInfoLastName")
                .HasMaxLength(100)
                .IsRequired();

            contactInfo.Property(c => c.EmailAddress)
                .HasColumnName("ContactInfoEmailAddress")
                .HasMaxLength(255)
                .IsRequired();

            contactInfo.Property(c => c.MobilePhone)
                .HasColumnName("ContactInfoMobilePhone")
                .HasMaxLength(20);
        });

        // Configure relationships
        builder.HasOne(t => t.Property)
            .WithMany(p => p.Tenants)
            .HasForeignKey("PropertyId")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint: One tenant per unit per property
        builder.HasIndex("PropertyId", "UnitNumber")
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Property_Unit");

        // Configure domain events to be ignored
        builder.Ignore(t => t.DomainEvents);
    }
}