using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;
using RentalRepairs.Domain.Specifications;

namespace RentalRepairs.Domain.Repositories;

public interface ITenantRequestRepository : IRepository<TenantRequest>
{
    Task<IEnumerable<TenantRequest>> GetBySpecificationAsync(ISpecification<TenantRequest> specification, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetByStatusAsync(TenantRequestStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetOverdueRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetByUrgencyLevelAsync(string urgencyLevel, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetByWorkerEmailAsync(string workerEmail, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantRequest>> GetByTenantIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string requestCode, CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(TenantRequestStatus status, CancellationToken cancellationToken = default);
}