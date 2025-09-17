using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Specifications;

namespace RentalRepairs.Domain.Repositories;

public interface IWorkerRepository : IRepository<Worker>
{
    Task<Worker?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Worker>> GetBySpecializationAsync(string specialization, CancellationToken cancellationToken = default);
    Task<IEnumerable<Worker>> GetActiveWorkersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Worker>> GetBySpecificationAsync(ISpecification<Worker> specification, CancellationToken cancellationToken = default);
}