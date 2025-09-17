using Microsoft.EntityFrameworkCore;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Property)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Property)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetByPropertyAndUnitAsync(int propertyId, string unitNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Property)
            .FirstOrDefaultAsync(t => t.Property.Id == propertyId && t.UnitNumber == unitNumber, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Property)
            .Where(t => t.Property.Id == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetWithActiveRequestsAsync(CancellationToken cancellationToken = default)
    {
        // Since there's no direct navigation from Tenant to TenantRequests,
        // we'll need to use a different approach or add the navigation property later
        // For now, return all tenants (can be enhanced later)
        return await _context.Tenants
            .Include(t => t.Property)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetBySpecificationAsync(ISpecification<Tenant> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsInUnitAsync(string propertyCode, string unitNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Property)
            .AnyAsync(t => t.Property.Code == propertyCode && t.UnitNumber == unitNumber, cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public void Update(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
    }

    public void Remove(Tenant tenant)
    {
        _context.Tenants.Remove(tenant);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Tenant> ApplySpecification(ISpecification<Tenant> specification)
    {
        var query = _context.Tenants.AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply string includes
        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        return query;
    }
}