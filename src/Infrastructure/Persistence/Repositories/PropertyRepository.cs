using Microsoft.EntityFrameworkCore;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.Infrastructure.Persistence.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Property?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Tenants)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetBySpecificationAsync(ISpecification<Property> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _context.Properties.AddAsync(property, cancellationToken);
    }

    public void Update(Property property)
    {
        _context.Properties.Update(property);
    }

    public void Remove(Property property)
    {
        _context.Properties.Remove(property);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AnyAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Tenants)
            .Where(p => p.Address.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetBySuperintendentEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Tenants)
            .Where(p => p.Superintendent.EmailAddress == email)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Property> ApplySpecification(ISpecification<Property> specification)
    {
        var query = _context.Properties.AsQueryable();

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