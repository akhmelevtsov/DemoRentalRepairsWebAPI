using RentalRepairs.Application.DTOs;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Application.Interfaces;

/// <summary>
/// Application service for tenant request management operations
/// </summary>
public interface ITenantRequestService
{
    // Request Lifecycle Management
    Task<int> CreateTenantRequestAsync(TenantRequestDto requestDto, CancellationToken cancellationToken = default);
    Task SubmitTenantRequestAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task ScheduleServiceWorkAsync(int tenantRequestId, DateTime scheduledDate, string workerEmail, string workOrderNumber, CancellationToken cancellationToken = default);
    Task ReportWorkCompletedAsync(int tenantRequestId, bool completedSuccessfully, string completionNotes, CancellationToken cancellationToken = default);
    Task CloseRequestAsync(int tenantRequestId, string closureNotes, CancellationToken cancellationToken = default);
    
    // Request Retrieval
    Task<TenantRequestDto> GetTenantRequestByIdAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task<List<TenantRequestDto>> GetTenantRequestsAsync(
        int? propertyId = null,
        int? tenantId = null,
        TenantRequestStatus? status = null,
        string? urgencyLevel = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool pendingOnly = false,
        bool overdueOnly = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    Task<List<TenantRequestDto>> GetWorkerRequestsAsync(
        string workerEmail,
        TenantRequestStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    Task<List<TenantRequestDto>> GetRequestsByPropertyAsync(
        string propertyCode,
        TenantRequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}