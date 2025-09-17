using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Common.Models;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.TenantRequests;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using Mapster;

namespace RentalRepairs.Application.Queries.TenantRequests.Handlers;

public class GetTenantRequestByIdQueryHandler : IQueryHandler<GetTenantRequestByIdQuery, TenantRequestDto>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public GetTenantRequestByIdQueryHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task<TenantRequestDto> Handle(GetTenantRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantRequest = await _tenantRequestRepository.GetByIdAsync(request.TenantRequestId, cancellationToken);
        
        if (tenantRequest == null)
        {
            throw new ArgumentException($"Tenant request with ID '{request.TenantRequestId}' not found");
        }

        return tenantRequest.Adapt<TenantRequestDto>();
    }
}

public class GetTenantRequestsQueryHandler : IQueryHandler<GetTenantRequestsQuery, PagedResult<TenantRequestDto>>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public GetTenantRequestsQueryHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task<PagedResult<TenantRequestDto>> Handle(GetTenantRequestsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.TenantRequest> tenantRequests;
        int totalCount;

        if (request.PendingOnly)
        {
            var pendingSpec = new PendingTenantRequestsSpecification();
            var allPending = await _tenantRequestRepository.GetBySpecificationAsync(pendingSpec, cancellationToken);
            totalCount = allPending.Count();
            tenantRequests = allPending.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (request.OverdueOnly)
        {
            var overdueSpec = new OverdueTenantRequestsSpecification(DateTime.UtcNow);
            var allOverdue = await _tenantRequestRepository.GetBySpecificationAsync(overdueSpec, cancellationToken);
            totalCount = allOverdue.Count();
            tenantRequests = allOverdue.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (request.Status.HasValue)
        {
            var statusSpec = new TenantRequestByStatusSpecification(request.Status.Value);
            var allByStatus = await _tenantRequestRepository.GetBySpecificationAsync(statusSpec, cancellationToken);
            totalCount = allByStatus.Count();
            tenantRequests = allByStatus.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (!string.IsNullOrEmpty(request.UrgencyLevel))
        {
            var urgencySpec = new TenantRequestsByUrgencySpecification(request.UrgencyLevel);
            var allByUrgency = await _tenantRequestRepository.GetBySpecificationAsync(urgencySpec, cancellationToken);
            totalCount = allByUrgency.Count();
            tenantRequests = allByUrgency.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (request.PropertyId.HasValue)
        {
            var allByProperty = await _tenantRequestRepository.GetByPropertyIdAsync(request.PropertyId.Value, cancellationToken);
            totalCount = allByProperty.Count();
            tenantRequests = allByProperty.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (request.TenantId.HasValue)
        {
            var allByTenant = await _tenantRequestRepository.GetByTenantIdAsync(request.TenantId.Value, cancellationToken);
            totalCount = allByTenant.Count();
            tenantRequests = allByTenant.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (!string.IsNullOrEmpty(request.WorkerEmail))
        {
            var allByWorker = await _tenantRequestRepository.GetByWorkerEmailAsync(request.WorkerEmail, cancellationToken);
            totalCount = allByWorker.Count();
            tenantRequests = allByWorker.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            var dateRangeSpec = new TenantRequestsByDateRangeSpecification(
                request.FromDate ?? DateTime.MinValue,
                request.ToDate ?? DateTime.MaxValue);
            var allByDateRange = await _tenantRequestRepository.GetBySpecificationAsync(dateRangeSpec, cancellationToken);
            totalCount = allByDateRange.Count();
            tenantRequests = allByDateRange.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
        else
        {
            var allRequests = await _tenantRequestRepository.GetAllAsync(cancellationToken);
            totalCount = allRequests.Count();
            tenantRequests = allRequests.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }

        var dtos = tenantRequests.Adapt<List<TenantRequestDto>>();
        return new PagedResult<TenantRequestDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}

public class GetWorkerRequestsQueryHandler : IQueryHandler<GetWorkerRequestsQuery, PagedResult<TenantRequestDto>>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public GetWorkerRequestsQueryHandler(ITenantRequestRepository tenantRequestRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task<PagedResult<TenantRequestDto>> Handle(GetWorkerRequestsQuery request, CancellationToken cancellationToken)
    {
        var allRequests = await _tenantRequestRepository.GetByWorkerEmailAsync(request.WorkerEmail, cancellationToken);

        // Apply filtering
        if (request.Status.HasValue)
        {
            allRequests = allRequests.Where(tr => tr.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            allRequests = allRequests.Where(tr => tr.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            allRequests = allRequests.Where(tr => tr.CreatedAt <= request.ToDate.Value);
        }

        var totalCount = allRequests.Count();
        
        // Apply pagination
        var paginatedRequests = allRequests
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var dtos = paginatedRequests.Adapt<List<TenantRequestDto>>();
        return new PagedResult<TenantRequestDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}

public class GetRequestsByPropertyQueryHandler : IQueryHandler<GetRequestsByPropertyQuery, PagedResult<TenantRequestDto>>
{
    private readonly ITenantRequestRepository _tenantRequestRepository;
    private readonly IPropertyRepository _propertyRepository;

    public GetRequestsByPropertyQueryHandler(
        ITenantRequestRepository tenantRequestRepository,
        IPropertyRepository propertyRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<TenantRequestDto>> Handle(GetRequestsByPropertyQuery request, CancellationToken cancellationToken)
    {
        // Get property by code to get the ID
        var property = await _propertyRepository.GetByCodeAsync(request.PropertyCode, cancellationToken);
        if (property == null)
        {
            throw new ArgumentException($"Property with code '{request.PropertyCode}' not found");
        }

        var allRequests = await _tenantRequestRepository.GetByPropertyIdAsync(property.Id, cancellationToken);

        // Apply status filtering
        if (request.Status.HasValue)
        {
            allRequests = allRequests.Where(tr => tr.Status == request.Status.Value);
        }

        var totalCount = allRequests.Count();

        // Apply pagination
        var paginatedRequests = allRequests
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var dtos = paginatedRequests.Adapt<List<TenantRequestDto>>();
        return new PagedResult<TenantRequestDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}