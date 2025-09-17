using MediatR;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Commands.Tenants;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Application.Queries.Properties;
using RentalRepairs.Application.Queries.Tenants;

namespace RentalRepairs.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IMediator _mediator;

    public PropertyService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> RegisterPropertyAsync(PropertyDto propertyDto, CancellationToken cancellationToken = default)
    {
        var command = new RegisterPropertyCommand
        {
            Name = propertyDto.Name,
            Code = propertyDto.Code,
            PhoneNumber = propertyDto.PhoneNumber,
            NoReplyEmailAddress = propertyDto.NoReplyEmailAddress,
            Units = propertyDto.Units,
            Address = propertyDto.Address,
            Superintendent = propertyDto.Superintendent
        };

        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<PropertyDto> GetPropertyByIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var query = new GetPropertyByIdQuery(propertyId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PropertyDto> GetPropertyByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var query = new GetPropertyByCodeQuery(code);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<PropertyDto>> GetPropertiesAsync(string? city = null, string? superintendentEmail = null, bool? withTenants = null, CancellationToken cancellationToken = default)
    {
        var query = new GetPropertiesQuery
        {
            City = city,
            SuperintendentEmail = superintendentEmail,
            WithTenants = withTenants,
            PageNumber = 1,
            PageSize = int.MaxValue // Get all properties
        };

        var result = await _mediator.Send(query, cancellationToken);
        return result.Items;
    }

    public async Task<PropertyStatisticsDto> GetPropertyStatisticsAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var query = new GetPropertyStatisticsQuery(propertyId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<int> RegisterTenantAsync(int propertyId, TenantDto tenantDto, CancellationToken cancellationToken = default)
    {
        var command = new RegisterTenantCommand
        {
            PropertyId = propertyId,
            ContactInfo = tenantDto.ContactInfo,
            UnitNumber = tenantDto.UnitNumber
        };

        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<TenantDto> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var query = new GetTenantByIdQuery(tenantId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<TenantDto> GetTenantByPropertyAndUnitAsync(int propertyId, string unitNumber, CancellationToken cancellationToken = default)
    {
        var query = new GetTenantByPropertyAndUnitQuery(propertyId, unitNumber);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<TenantDto>> GetTenantsByPropertyAsync(int propertyId, bool withActiveRequestsOnly = false, CancellationToken cancellationToken = default)
    {
        var query = new GetTenantsByPropertyQuery(propertyId)
        {
            WithActiveRequestsOnly = withActiveRequestsOnly
        };

        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<bool> IsUnitAvailableAsync(string propertyCode, string unitNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var property = await GetPropertyByCodeAsync(propertyCode, cancellationToken);
            var tenant = await GetTenantByPropertyAndUnitAsync(property.Id, unitNumber, cancellationToken);
            return tenant == null; // Unit is available if no tenant found
        }
        catch (ArgumentException)
        {
            // If tenant not found, unit is available
            return true;
        }
    }

    public async Task<List<string>> GetAvailableUnitsAsync(string propertyCode, CancellationToken cancellationToken = default)
    {
        var property = await GetPropertyByCodeAsync(propertyCode, cancellationToken);
        var tenants = await GetTenantsByPropertyAsync(property.Id, cancellationToken: cancellationToken);
        
        var occupiedUnits = tenants.Select(t => t.UnitNumber).ToHashSet();
        var availableUnits = property.Units.Where(unit => !occupiedUnits.Contains(unit)).ToList();
        
        return availableUnits;
    }
}