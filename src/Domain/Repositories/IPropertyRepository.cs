using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Specifications;

namespace RentalRepairs.Domain.Repositories;

public interface IPropertyRepository : IRepository<Property>
{
    Task<Property?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetBySpecificationAsync(ISpecification<Property> specification, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetBySuperintendentEmailAsync(string email, CancellationToken cancellationToken = default);
}