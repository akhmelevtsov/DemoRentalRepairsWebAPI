using RentalRepairs.Domain.Common;

namespace RentalRepairs.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}