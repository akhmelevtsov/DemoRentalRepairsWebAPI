using Microsoft.EntityFrameworkCore;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.Infrastructure.Persistence.Repositories;

public class TenantRequestRepository : ITenantRequestRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TenantRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetBySpecificationAsync(ISpecification<TenantRequest> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetByStatusAsync(TenantRequestStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.Status == TenantRequestStatus.Submitted || tr.Status == TenantRequestStatus.Scheduled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetOverdueRequestsAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-7); // Consider requests older than 7 days as overdue
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.Status == TenantRequestStatus.Scheduled && tr.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetByUrgencyLevelAsync(string urgencyLevel, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.UrgencyLevel == urgencyLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetByWorkerEmailAsync(string workerEmail, CancellationToken cancellationToken = default)
    {
        // Since AssignedWorkerEmail doesn't exist as a persistent property, 
        // this method will return empty for now. Can be enhanced with domain events or separate tracking
        return new List<TenantRequest>();
    }

    public async Task<IEnumerable<TenantRequest>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.Tenant.Property.Id == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .Include(tr => tr.Tenant)
                .ThenInclude(t => t.Property)
            .Where(tr => tr.Tenant.Id == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string requestCode, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .AnyAsync(tr => tr.Code == requestCode, cancellationToken);
    }

    public async Task<int> CountByStatusAsync(TenantRequestStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.TenantRequests
            .CountAsync(tr => tr.Status == status, cancellationToken);
    }

    public async Task AddAsync(TenantRequest tenantRequest, CancellationToken cancellationToken = default)
    {
        await _context.TenantRequests.AddAsync(tenantRequest, cancellationToken);
    }

    public void Update(TenantRequest tenantRequest)
    {
        _context.TenantRequests.Update(tenantRequest);
    }

    public void Remove(TenantRequest tenantRequest)
    {
        _context.TenantRequests.Remove(tenantRequest);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TenantRequest> ApplySpecification(ISpecification<TenantRequest> specification)
    {
        var query = _context.TenantRequests.AsQueryable();

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