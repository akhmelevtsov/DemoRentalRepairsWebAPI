using RentalRepairs.Domain.Repositories;

namespace RentalRepairs.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    ITenantRepository Tenants { get; }
    ITenantRequestRepository TenantRequests { get; }
    IWorkerRepository Workers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}