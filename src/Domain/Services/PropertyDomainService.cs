using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Services;

public class PropertyDomainService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly DomainValidationService _validationService;

    public PropertyDomainService(
        IPropertyRepository propertyRepository,
        ITenantRepository tenantRepository,
        DomainValidationService validationService)
    {
        _propertyRepository = propertyRepository;
        _tenantRepository = tenantRepository;
        _validationService = validationService;
    }

    public async Task<bool> IsPropertyCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
    {
        return !await _propertyRepository.ExistsAsync(code, cancellationToken);
    }

    public async Task<bool> IsUnitAvailableAsync(string propertyCode, string unitNumber, CancellationToken cancellationToken = default)
    {
        return !await _tenantRepository.ExistsInUnitAsync(propertyCode, unitNumber, cancellationToken);
    }

    public async Task<Property> CreatePropertyAsync(
        string name,
        string code,
        PropertyAddress address,
        string phoneNumber,
        PersonContactInfo superintendent,
        List<string> units,
        string noReplyEmailAddress,
        CancellationToken cancellationToken = default)
    {
        // Validate business rules
        await ValidatePropertyRegistrationAsync(code, units, cancellationToken);

        // Create the property
        var property = new Property(name, code, address, phoneNumber, superintendent, units, noReplyEmailAddress);

        // Validate the entity
        await _validationService.ValidateAsync(property, cancellationToken);

        return property;
    }

    public async Task ValidatePropertyRegistrationAsync(
        string code, 
        List<string> units, 
        CancellationToken cancellationToken = default)
    {
        if (!await IsPropertyCodeUniqueAsync(code, cancellationToken))
        {
            throw new PropertyDomainException($"Property with code '{code}' already exists");
        }

        if (units == null || !units.Any())
        {
            throw new PropertyDomainException("Property must have at least one unit");
        }

        if (units.Distinct().Count() != units.Count)
        {
            throw new PropertyDomainException("Property cannot have duplicate unit numbers");
        }

        // Validate unit number format
        foreach (var unit in units)
        {
            if (!IsValidUnitNumber(unit))
            {
                throw new PropertyDomainException($"Unit number '{unit}' has invalid format");
            }
        }
    }

    public async Task<Property> GetPropertyWithTenantsAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var specification = new PropertyWithTenantsSpecification();
        var properties = await _propertyRepository.GetBySpecificationAsync(specification, cancellationToken);
        
        var property = properties.FirstOrDefault(p => p.Id == propertyId);
        if (property == null)
        {
            throw new PropertyDomainException($"Property with ID '{propertyId}' not found");
        }

        return property;
    }

    public async Task<IEnumerable<Property>> GetPropertiesBySuperintendentAsync(
        string superintendentEmail, 
        CancellationToken cancellationToken = default)
    {
        var specification = new PropertiesBySuperintendentEmailSpecification(superintendentEmail);
        return await _propertyRepository.GetBySpecificationAsync(specification, cancellationToken);
    }

    public async Task<Tenant> RegisterTenantAsync(
        int propertyId, 
        PersonContactInfo contactInfo,
        string unitNumber, 
        CancellationToken cancellationToken = default)
    {
        // Validate business rules
        await ValidateTenantRegistrationAsync(propertyId, unitNumber, cancellationToken);

        // Get the property
        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property == null)
        {
            throw new PropertyDomainException($"Property with ID '{propertyId}' not found");
        }

        // Register the tenant through the property aggregate
        var tenant = property.RegisterTenant(contactInfo, unitNumber);

        // Validate the entity
        await _validationService.ValidateAsync(tenant, cancellationToken);

        return tenant;
    }

    public async Task ValidateTenantRegistrationAsync(
        int propertyId, 
        string unitNumber, 
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property == null)
        {
            throw new PropertyDomainException($"Property with ID '{propertyId}' not found");
        }

        if (!property.Units.Contains(unitNumber))
        {
            throw new PropertyDomainException($"Unit '{unitNumber}' does not exist in property '{property.Code}'");
        }

        if (!await IsUnitAvailableAsync(property.Code, unitNumber, cancellationToken))
        {
            throw new PropertyDomainException($"Unit '{unitNumber}' is already occupied in property '{property.Code}'");
        }
    }

    public async Task<IEnumerable<string>> GetAvailableUnitsAsync(
        int propertyId, 
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property == null)
        {
            throw new PropertyDomainException($"Property with ID '{propertyId}' not found");
        }

        var occupiedUnitsSpec = new TenantByPropertySpecification(propertyId);
        var tenants = await _tenantRepository.GetBySpecificationAsync(occupiedUnitsSpec, cancellationToken);
        var occupiedUnits = tenants.Select(t => t.UnitNumber).ToHashSet();

        return property.Units.Where(unit => !occupiedUnits.Contains(unit));
    }

    public async Task<Dictionary<string, object>> GetPropertyStatisticsAsync(
        int propertyId,
        CancellationToken cancellationToken = default)
    {
        var property = await GetPropertyWithTenantsAsync(propertyId, cancellationToken);
        var availableUnits = await GetAvailableUnitsAsync(propertyId, cancellationToken);

        return new Dictionary<string, object>
        {
            ["PropertyName"] = property.Name,
            ["PropertyCode"] = property.Code,
            ["TotalUnits"] = property.Units.Count,
            ["OccupiedUnits"] = property.Tenants.Count,
            ["AvailableUnits"] = availableUnits.Count(),
            ["OccupancyRate"] = property.Units.Count > 0 ? (double)property.Tenants.Count / property.Units.Count : 0,
            ["SuperintendentName"] = property.Superintendent.GetFullName(),
            ["SuperintendentEmail"] = property.Superintendent.EmailAddress
        };
    }

    private static bool IsValidUnitNumber(string unitNumber)
    {
        // Unit number should be alphanumeric, possibly with dash or space
        return !string.IsNullOrWhiteSpace(unitNumber) && 
               unitNumber.Length <= 10 && 
               System.Text.RegularExpressions.Regex.IsMatch(unitNumber, @"^[A-Za-z0-9\-\s]+$");
    }
}