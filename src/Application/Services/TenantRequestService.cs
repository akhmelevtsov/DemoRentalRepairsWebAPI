using MediatR;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Application.Queries.TenantRequests;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Application.Services;

public class TenantRequestService : ITenantRequestService
{
    private readonly IMediator _mediator;

    public TenantRequestService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> CreateTenantRequestAsync(TenantRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        var command = new CreateTenantRequestCommand
        {
            TenantId = requestDto.TenantId,
            Title = requestDto.Title,
            Description = requestDto.Description,
            UrgencyLevel = requestDto.UrgencyLevel
        };

        return await _mediator.Send(command, cancellationToken);
    }

    public async Task SubmitTenantRequestAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var command = new SubmitTenantRequestCommand
        {
            TenantRequestId = tenantRequestId
        };

        await _mediator.Send(command, cancellationToken);
    }

    public async Task ScheduleServiceWorkAsync(int tenantRequestId, DateTime scheduledDate, string workerEmail, string workOrderNumber, CancellationToken cancellationToken = default)
    {
        var command = new ScheduleServiceWorkCommand
        {
            TenantRequestId = tenantRequestId,
            ScheduledDate = scheduledDate,
            WorkerEmail = workerEmail,
            WorkOrderNumber = workOrderNumber
        };

        await _mediator.Send(command, cancellationToken);
    }

    public async Task ReportWorkCompletedAsync(int tenantRequestId, bool completedSuccessfully, string completionNotes, CancellationToken cancellationToken = default)
    {
        var command = new ReportWorkCompletedCommand
        {
            TenantRequestId = tenantRequestId,
            CompletedSuccessfully = completedSuccessfully,
            CompletionNotes = completionNotes
        };

        await _mediator.Send(command, cancellationToken);
    }

    public async Task CloseRequestAsync(int tenantRequestId, string closureNotes, CancellationToken cancellationToken = default)
    {
        var command = new CloseRequestCommand
        {
            TenantRequestId = tenantRequestId,
            ClosureNotes = closureNotes
        };

        await _mediator.Send(command, cancellationToken);
    }

    public async Task<TenantRequestDto> GetTenantRequestByIdAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var query = new GetTenantRequestByIdQuery(tenantRequestId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<TenantRequestDto>> GetTenantRequestsAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantRequestsQuery
        {
            PropertyId = propertyId,
            TenantId = tenantId,
            Status = status,
            UrgencyLevel = urgencyLevel,
            FromDate = fromDate,
            ToDate = toDate,
            PendingOnly = pendingOnly,
            OverdueOnly = overdueOnly,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return result.Items;
    }

    public async Task<List<TenantRequestDto>> GetWorkerRequestsAsync(
        string workerEmail,
        TenantRequestStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetWorkerRequestsQuery(workerEmail)
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return result.Items;
    }

    public async Task<List<TenantRequestDto>> GetRequestsByPropertyAsync(
        string propertyCode,
        TenantRequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRequestsByPropertyQuery(propertyCode)
        {
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return result.Items;
    }
}