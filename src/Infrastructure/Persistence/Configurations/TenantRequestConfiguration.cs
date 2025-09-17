using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Infrastructure.Persistence.Configurations;

public class TenantRequestConfiguration : IEntityTypeConfiguration<TenantRequest>
{
    public void Configure(EntityTypeBuilder<TenantRequest> builder)
    {
        builder.ToTable("TenantRequests");

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(tr => tr.Code)
            .IsUnique();

        builder.Property(tr => tr.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tr => tr.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tr => tr.UrgencyLevel)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(tr => tr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(tr => tr.ServiceWorkOrderCount)
            .IsRequired();

        // Configure the explicit TenantId foreign key property
        builder.Property(tr => tr.TenantId)
            .IsRequired();

        // Configure the relationship to Tenant
        builder.HasOne(tr => tr.Tenant)
            .WithMany()
            .HasForeignKey(tr => tr.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes for common queries
        builder.HasIndex(tr => tr.Status)
            .HasDatabaseName("IX_TenantRequests_Status");

        builder.HasIndex(tr => tr.UrgencyLevel)
            .HasDatabaseName("IX_TenantRequests_UrgencyLevel");

        builder.HasIndex(tr => tr.TenantId)
            .HasDatabaseName("IX_TenantRequests_TenantId");

        builder.HasIndex(tr => tr.CreatedAt)
            .HasDatabaseName("IX_TenantRequests_CreatedAt");

        // Configure domain events to be ignored
        builder.Ignore(tr => tr.DomainEvents);

        // Ignore computed properties - these are calculated from navigation properties
        builder.Ignore(tr => tr.TenantFullName);
        builder.Ignore(tr => tr.PropertyName);
        builder.Ignore(tr => tr.SuperintendentFullName);
        builder.Ignore(tr => tr.PropertyId);
        builder.Ignore(tr => tr.TenantIdentifier); // Updated property name
        builder.Ignore(tr => tr.TenantUnit);
        builder.Ignore(tr => tr.PropertyNoReplyEmail);
        builder.Ignore(tr => tr.TenantEmail);
        builder.Ignore(tr => tr.PropertyPhone);
        builder.Ignore(tr => tr.SuperintendentEmail);

        // Ignore the request changes collection - will be configured separately if needed
        builder.Ignore(tr => tr.RequestChanges);
    }
}