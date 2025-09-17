using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.Tenants;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using Mapster;

namespace RentalRepairs.Application.Queries.Tenants.Handlers;

public class GetTenantByIdQueryHandler : IQueryHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        
        if (tenant == null)
        {
            throw new ArgumentException($"Tenant with ID '{request.TenantId}' not found");
        }

        return tenant.Adapt<TenantDto>();
    }
}

public class GetTenantByPropertyAndUnitQueryHandler : IQueryHandler<GetTenantByPropertyAndUnitQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByPropertyAndUnitQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto> Handle(GetTenantByPropertyAndUnitQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPropertyAndUnitAsync(request.PropertyId, request.UnitNumber, cancellationToken);
        
        if (tenant == null)
        {
            throw new ArgumentException($"Tenant not found for property ID '{request.PropertyId}' and unit '{request.UnitNumber}'");
        }

        return tenant.Adapt<TenantDto>();
    }
}

public class GetTenantsByPropertyQueryHandler : IQueryHandler<GetTenantsByPropertyQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsByPropertyQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantDto>> Handle(GetTenantsByPropertyQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Tenant> tenants;

        if (request.WithActiveRequestsOnly)
        {
            // Get tenants with active requests, then filter by property
            var activeRequestsSpec = new TenantsWithActiveRequestsSpecification();
            var allTenantsWithActiveRequests = await _tenantRepository.GetBySpecificationAsync(activeRequestsSpec, cancellationToken);
            tenants = allTenantsWithActiveRequests.Where(t => t.Property.Id == request.PropertyId);
        }
        else
        {
            tenants = await _tenantRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
        }

        return tenants.Adapt<List<TenantDto>>();
    }
}