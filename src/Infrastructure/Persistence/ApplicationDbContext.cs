using Microsoft.EntityFrameworkCore;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Entities;
using System.Reflection;

namespace RentalRepairs.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTime? _dateTime;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null,
        IDateTime? dateTime = null) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    // DbSets for Domain Entities
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantRequest> TenantRequests => Set<TenantRequest>();
    public DbSet<Worker> Workers => Set<Worker>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Only apply auditing if services are available (not in test scenarios)
        if (_currentUserService != null && _dateTime != null)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedAt = _dateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = _currentUserService.UserId;
                        entry.Entity.UpdatedAt = _dateTime.UtcNow;
                        break;
                }
            }
        }
        else
        {
            // For tests, use default values
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = "test-user";
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = "test-user";
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        await DispatchDomainEvents();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    private async Task DispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        var domainEvents = domainEventEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEventEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // In future steps, we'll publish these through MediatR
            // await _mediator.Publish(domainEvent);
        }
    }
}