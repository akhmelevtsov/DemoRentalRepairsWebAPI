using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Common.Models;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.Properties;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Specifications;
using Mapster;

namespace RentalRepairs.Application.Queries.Properties.Handlers;

public class GetPropertyByIdQueryHandler : IQueryHandler<GetPropertyByIdQuery, PropertyDto>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        
        if (property == null)
        {
            throw new ArgumentException($"Property with ID '{request.PropertyId}' not found");
        }

        return property.Adapt<PropertyDto>();
    }
}

public class GetPropertyByCodeQueryHandler : IQueryHandler<GetPropertyByCodeQuery, PropertyDto>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByCodeQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto> Handle(GetPropertyByCodeQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByCodeAsync(request.Code, cancellationToken);
        
        if (property == null)
        {
            throw new ArgumentException($"Property with code '{request.Code}' not found");
        }

        return property.Adapt<PropertyDto>();
    }
}

public class GetPropertiesQueryHandler : IQueryHandler<GetPropertiesQuery, PagedResult<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Property> allProperties;

        if (!string.IsNullOrEmpty(request.City))
        {
            var citySpec = new PropertiesByCitySpecification(request.City);
            allProperties = await _propertyRepository.GetBySpecificationAsync(citySpec, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.SuperintendentEmail))
        {
            var superintendentSpec = new PropertiesBySuperintendentEmailSpecification(request.SuperintendentEmail);
            allProperties = await _propertyRepository.GetBySpecificationAsync(superintendentSpec, cancellationToken);
        }
        else if (request.WithTenants.HasValue)
        {
            if (request.WithTenants.Value)
            {
                var withTenantsSpec = new PropertyWithTenantsSpecification();
                allProperties = await _propertyRepository.GetBySpecificationAsync(withTenantsSpec, cancellationToken);
            }
            else
            {
                allProperties = await _propertyRepository.GetAllAsync(cancellationToken);
            }
        }
        else
        {
            allProperties = await _propertyRepository.GetAllAsync(cancellationToken);
        }

        var totalCount = allProperties.Count();

        // Apply pagination
        var pagedProperties = allProperties
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var dtos = pagedProperties.Adapt<List<PropertyDto>>();
        return new PagedResult<PropertyDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}

public class GetPropertyStatisticsQueryHandler : IQueryHandler<GetPropertyStatisticsQuery, PropertyStatisticsDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ITenantRepository _tenantRepository;

    public GetPropertyStatisticsQueryHandler(
        IPropertyRepository propertyRepository,
        ITenantRepository tenantRepository)
    {
        _propertyRepository = propertyRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<PropertyStatisticsDto> Handle(GetPropertyStatisticsQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null)
        {
            throw new ArgumentException($"Property with ID '{request.PropertyId}' not found");
        }

        var tenants = await _tenantRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
        var totalUnits = property.Units.Count;
        var occupiedUnits = tenants.Count();
        var availableUnits = totalUnits - occupiedUnits;
        var occupancyRate = totalUnits > 0 ? (double)occupiedUnits / totalUnits : 0;

        return new PropertyStatisticsDto
        {
            PropertyName = property.Name,
            PropertyCode = property.Code,
            TotalUnits = totalUnits,
            OccupiedUnits = occupiedUnits,
            AvailableUnits = availableUnits,
            OccupancyRate = occupancyRate,
            SuperintendentName = property.Superintendent.GetFullName(),
            SuperintendentEmail = property.Superintendent.EmailAddress
        };
    }
}