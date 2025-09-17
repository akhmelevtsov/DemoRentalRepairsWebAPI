using Microsoft.EntityFrameworkCore;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.Infrastructure.Persistence.Repositories;

public class WorkerRepository : IWorkerRepository
{
    private readonly ApplicationDbContext _context;

    public WorkerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Worker?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Workers
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Worker>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Workers
            .ToListAsync(cancellationToken);
    }

    public async Task<Worker?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Workers
            .FirstOrDefaultAsync(w => w.ContactInfo.EmailAddress == email, cancellationToken);
    }

    public async Task<IEnumerable<Worker>> GetBySpecializationAsync(string specialization, CancellationToken cancellationToken = default)
    {
        return await _context.Workers
            .Where(w => w.Specialization == specialization)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Worker>> GetActiveWorkersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Workers
            .Where(w => w.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Worker>> GetBySpecificationAsync(ISpecification<Worker> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Worker worker, CancellationToken cancellationToken = default)
    {
        await _context.Workers.AddAsync(worker, cancellationToken);
    }

    public void Update(Worker worker)
    {
        _context.Workers.Update(worker);
    }

    public void Remove(Worker worker)
    {
        _context.Workers.Remove(worker);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Worker> ApplySpecification(ISpecification<Worker> specification)
    {
        var query = _context.Workers.AsQueryable();

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