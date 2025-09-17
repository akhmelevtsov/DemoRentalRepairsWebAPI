using MediatR;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Exceptions;

namespace RentalRepairs.Application.Commands.TenantRequests.Handlers;

public class CreateTenantRequestCommandHandler : ICommandHandler<CreateTenantRequestCommand, int>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantRequestRepository _tenantRequestRepository;
    private readonly TenantRequestDomainService _tenantRequestDomainService;

    public CreateTenantRequestCommandHandler(
        ITenantRepository tenantRepository,
        ITenantRequestRepository tenantRequestRepository,
        TenantRequestDomainService tenantRequestDomainService)
    {
        _tenantRepository = tenantRepository;
        _tenantRequestRepository = tenantRequestRepository;
        _tenantRequestDomainService = tenantRequestDomainService;
    }

    public async Task<int> Handle(CreateTenantRequestCommand request, CancellationToken cancellationToken)
    {
        // Get the tenant
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new TenantRequestDomainException($"Tenant with ID '{request.TenantId}' not found");
        }

        // Create the request using the tenant aggregate
        var tenantRequest = tenant.CreateRequest(
            request.Title,
            request.Description,
            request.UrgencyLevel);

        // Save to repository
        await _tenantRequestRepository.AddAsync(tenantRequest, cancellationToken);
        await _tenantRequestRepository.SaveChangesAsync(cancellationToken);

        return tenantRequest.Id;
    }
}

public class SubmitTenantRequestCommandHandler : ICommandHandler<SubmitTenantRequestCommand>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public SubmitTenantRequestCommandHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task Handle(SubmitTenantRequestCommand request, CancellationToken cancellationToken)
    {
        var tenantRequest = await _tenantRequestRepository.GetByIdAsync(request.TenantRequestId, cancellationToken);
        if (tenantRequest == null)
        {
            throw new TenantRequestDomainException($"Tenant request with ID '{request.TenantRequestId}' not found");
        }

        // Submit the request (this will trigger domain events)
        tenantRequest.Submit();

        await _tenantRequestRepository.SaveChangesAsync(cancellationToken);
    }
}

public class ScheduleServiceWorkCommandHandler : ICommandHandler<ScheduleServiceWorkCommand>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly WorkerAssignmentService _workerAssignmentService;

    public ScheduleServiceWorkCommandHandler(
        ITenantRequestRepository tenantRequestRepository,
        IWorkerRepository workerRepository,
        WorkerAssignmentService workerAssignmentService)
    {
        _tenantRequestRepository = tenantRequestRepository;
        _workerRepository = workerRepository;
        _workerAssignmentService = workerAssignmentService;
    }

    public async Task Handle(ScheduleServiceWorkCommand request, CancellationToken cancellationToken)
    {
        var tenantRequest = await _tenantRequestRepository.GetByIdAsync(request.TenantRequestId, cancellationToken);
        if (tenantRequest == null)
        {
            throw new TenantRequestDomainException($"Tenant request with ID '{request.TenantRequestId}' not found");
        }

        // Verify worker exists and is available
        var worker = await _workerRepository.GetByEmailAsync(request.WorkerEmail, cancellationToken);
        if (worker == null)
        {
            throw new TenantRequestDomainException($"Worker with email '{request.WorkerEmail}' not found");
        }

        if (!worker.IsActive)
        {
            throw new TenantRequestDomainException($"Worker '{request.WorkerEmail}' is not active");
        }

        // Check if worker is available for the scheduled date
        var isAvailable = await _workerAssignmentService.IsWorkerAvailableAsync(worker, request.ScheduledDate, cancellationToken);
        if (!isAvailable)
        {
            throw new TenantRequestDomainException($"Worker '{request.WorkerEmail}' is not available on {request.ScheduledDate:yyyy-MM-dd}");
        }

        // Schedule the work
        tenantRequest.Schedule(request.ScheduledDate, request.WorkerEmail, request.WorkOrderNumber);

        await _tenantRequestRepository.SaveChangesAsync(cancellationToken);
    }
}

public class ReportWorkCompletedCommandHandler : ICommandHandler<ReportWorkCompletedCommand>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public ReportWorkCompletedCommandHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task Handle(ReportWorkCompletedCommand request, CancellationToken cancellationToken)
    {
        var tenantRequest = await _tenantRequestRepository.GetByIdAsync(request.TenantRequestId, cancellationToken);
        if (tenantRequest == null)
        {
            throw new TenantRequestDomainException($"Tenant request with ID '{request.TenantRequestId}' not found");
        }

        // Report work completion
        tenantRequest.ReportWorkCompleted(request.CompletedSuccessfully, request.CompletionNotes);

        await _tenantRequestRepository.SaveChangesAsync(cancellationToken);
    }
}

public class CloseRequestCommandHandler : ICommandHandler<CloseRequestCommand>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public CloseRequestCommandHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task Handle(CloseRequestCommand request, CancellationToken cancellationToken)
    {
        var tenantRequest = await _tenantRequestRepository.GetByIdAsync(request.TenantRequestId, cancellationToken);
        if (tenantRequest == null)
        {
            throw new TenantRequestDomainException($"Tenant request with ID '{request.TenantRequestId}' not found");
        }

        // Close the request
        tenantRequest.Close(request.ClosureNotes);

        await _tenantRequestRepository.SaveChangesAsync(cancellationToken);
    }
}