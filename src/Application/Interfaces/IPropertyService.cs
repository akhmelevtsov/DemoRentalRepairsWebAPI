using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.Properties;

namespace RentalRepairs.Application.Interfaces;

/// <summary>
/// Application service for property management operations
/// </summary>
public interface IPropertyService
{
    // Property Management
    Task<int> RegisterPropertyAsync(PropertyDto propertyDto, CancellationToken cancellationToken = default);
    Task<PropertyDto> GetPropertyByIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<PropertyDto> GetPropertyByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<PropertyDto>> GetPropertiesAsync(string? city = null, string? superintendentEmail = null, bool? withTenants = null, CancellationToken cancellationToken = default);
    Task<PropertyStatisticsDto> GetPropertyStatisticsAsync(int propertyId, CancellationToken cancellationToken = default);
    
    // Tenant Management within Property
    Task<int> RegisterTenantAsync(int propertyId, TenantDto tenantDto, CancellationToken cancellationToken = default);
    Task<TenantDto> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<TenantDto> GetTenantByPropertyAndUnitAsync(int propertyId, string unitNumber, CancellationToken cancellationToken = default);
    Task<List<TenantDto>> GetTenantsByPropertyAsync(int propertyId, bool withActiveRequestsOnly = false, CancellationToken cancellationToken = default);
    
    // Property Business Operations
    Task<bool> IsUnitAvailableAsync(string propertyCode, string unitNumber, CancellationToken cancellationToken = default);
    Task<List<string>> GetAvailableUnitsAsync(string propertyCode, CancellationToken cancellationToken = default);
}