using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Services;

public class TenantRequestDomainService
{
    private readonly ITenantRequestRepository _tenantRequestRepository;
    private readonly IWorkerRepository _workerRepository;

    public TenantRequestDomainService(
        ITenantRequestRepository tenantRequestRepository,
        IWorkerRepository workerRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
        _workerRepository = workerRepository;
    }

    public async Task<bool> IsRequestCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
    {
        return !await _tenantRequestRepository.ExistsAsync(code, cancellationToken);
    }

    public async Task<Worker> ValidateWorkerForSchedulingAsync(string workerEmail, CancellationToken cancellationToken = default)
    {
        var worker = await _workerRepository.GetByEmailAsync(workerEmail, cancellationToken);
        if (worker == null)
        {
            throw new TenantRequestDomainException($"Worker with email '{workerEmail}' not found");
        }

        if (!worker.IsActive)
        {
            throw new TenantRequestDomainException($"Worker '{workerEmail}' is not active");
        }

        return worker;
    }

    public async Task<IEnumerable<TenantRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        var specification = new PendingTenantRequestsSpecification();
        return await _tenantRequestRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetOverdueRequestsAsync(int daysOverdue = 7, CancellationToken cancellationToken = default)
    {
        var overdueDate = DateTime.UtcNow.AddDays(-daysOverdue);
        var specification = new OverdueTenantRequestsSpecification(overdueDate);
        return await _tenantRequestRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetRequestsByUrgencyAsync(string urgencyLevel, CancellationToken cancellationToken = default)
    {
        var specification = new TenantRequestsByUrgencySpecification(urgencyLevel);
        return await _tenantRequestRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetRequestsForPropertyAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var specification = new TenantRequestByPropertySpecification(propertyId);
        return await _tenantRequestRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<TenantRequest>> GetRequestsInDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        var specification = new TenantRequestsByDateRangeSpecification(startDate, endDate);
        return await _tenantRequestRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<Dictionary<TenantRequestStatus, int>> GetRequestStatusSummaryAsync(CancellationToken cancellationToken = default)
    {
        var statusCounts = new Dictionary<TenantRequestStatus, int>();

        foreach (TenantRequestStatus status in Enum.GetValues<TenantRequestStatus>())
        {
            var count = await _tenantRequestRepository.CountByStatusAsync(status, cancellationToken);
            statusCounts[status] = count;
        }

        return statusCounts;
    }

    public string GenerateRequestCode(string propertyCode, string unitNumber, int requestNumber)
    {
        return $"{propertyCode}-{unitNumber}-{requestNumber:D4}";
    }

    public string GenerateWorkOrderNumber(string requestCode, int workOrderSequence)
    {
        return $"WO-{requestCode}-{workOrderSequence:D2}";
    }

    public async Task ValidateRequestSchedulingAsync(
        TenantRequest request, 
        DateTime serviceDate, 
        string workerEmail, 
        CancellationToken cancellationToken = default)
    {
        if (request.Status != TenantRequestStatus.Submitted)
        {
            throw new TenantRequestDomainException($"Request '{request.Code}' cannot be scheduled. Current status: {request.Status}");
        }

        if (serviceDate <= DateTime.UtcNow)
        {
            throw new TenantRequestDomainException("Service date must be in the future");
        }

        await ValidateWorkerForSchedulingAsync(workerEmail, cancellationToken);
    }
}