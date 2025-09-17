using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Infrastructure.Persistence.Configurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.ToTable("Workers");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Specialization)
            .HasMaxLength(100);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure PersonContactInfo value object
        builder.OwnsOne(w => w.ContactInfo, contactInfo =>
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

            // Unique constraint on email address (within the owned entity)
            contactInfo.HasIndex(c => c.EmailAddress)
                .IsUnique()
                .HasDatabaseName("IX_Workers_Email");
        });

        // Index on specialization for efficient filtering
        builder.HasIndex(w => w.Specialization)
            .HasDatabaseName("IX_Workers_Specialization");

        // Index on IsActive for performance
        builder.HasIndex(w => w.IsActive)
            .HasDatabaseName("IX_Workers_IsActive");

        // Configure domain events to be ignored
        builder.Ignore(w => w.DomainEvents);
    }
}