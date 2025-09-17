using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Common.Models;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Application.Queries.TenantRequests;

public class GetTenantRequestByIdQuery : IQuery<TenantRequestDto>
{
    public int TenantRequestId { get; set; }

    public GetTenantRequestByIdQuery(int tenantRequestId)
    {
        TenantRequestId = tenantRequestId;
    }
}

public class GetTenantRequestsQuery : IQuery<PagedResult<TenantRequestDto>>
{
    public int? PropertyId { get; set; }
    public int? TenantId { get; set; }
    public string? WorkerEmail { get; set; }
    public TenantRequestStatus? Status { get; set; }
    public string? UrgencyLevel { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool PendingOnly { get; set; }
    public bool OverdueOnly { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetWorkerRequestsQuery : IQuery<PagedResult<TenantRequestDto>>
{
    public string WorkerEmail { get; set; } = default!;
    public TenantRequestStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetWorkerRequestsQuery(string workerEmail)
    {
        WorkerEmail = workerEmail;
    }
}

public class GetRequestsByPropertyQuery : IQuery<PagedResult<TenantRequestDto>>
{
    public string PropertyCode { get; set; } = default!;
    public TenantRequestStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetRequestsByPropertyQuery(string propertyCode)
    {
        PropertyCode = propertyCode;
    }
}