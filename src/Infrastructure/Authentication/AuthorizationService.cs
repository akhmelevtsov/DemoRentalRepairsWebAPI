using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Interfaces;
using System.Security.Claims;

namespace RentalRepairs.Infrastructure.Authentication;

/// <summary>
/// Authorization service implementation for role-based access control
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IPropertyService _propertyService;
    private readonly ITenantRequestService _tenantRequestService;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(
        IPropertyService propertyService,
        ITenantRequestService tenantRequestService,
        ILogger<AuthorizationService> logger)
    {
        _propertyService = propertyService;
        _tenantRequestService = tenantRequestService;
        _logger = logger;
    }

    public async Task<bool> CanAccessPropertyAsync(string userId, int propertyId, CancellationToken cancellationToken = default)
    {
        try
        {
            // System admin can access all properties
            if (await IsSystemAdminAsync(userId))
                return true;

            // Property superintendent can access their properties
            if (await IsPropertySuperintendentAsync(userId, propertyId, cancellationToken))
                return true;

            // Tenants can access their property
            if (int.TryParse(userId, out var tenantId))
            {
                var tenant = await _propertyService.GetTenantByIdAsync(tenantId, cancellationToken);
                if (tenant?.PropertyId == propertyId)
                    return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check property access for user {UserId} and property {PropertyId}", userId, propertyId);
            return false;
        }
    }

    public async Task<bool> CanAccessTenantRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default)
    {
        try
        {
            // System admin can access all requests
            if (await IsSystemAdminAsync(userId))
                return true;

            var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
            if (request == null)
                return false;

            // Tenant can access their own requests
            if (await IsTenantForRequestAsync(userId, tenantRequestId, cancellationToken))
                return true;

            // Property superintendent can access requests for their properties
            var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
            if (tenant != null && 
                await IsPropertySuperintendentAsync(userId, tenant.PropertyId, cancellationToken))
                return true;

            // Worker can access assigned requests
            if (await IsWorkerAssignedToRequestAsync(userId, tenantRequestId, cancellationToken))
                return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check tenant request access for user {UserId} and request {RequestId}", userId, tenantRequestId);
            return false;
        }
    }

    public async Task<bool> CanManageWorkersAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Only system admin can manage workers in this simplified model
            return await IsSystemAdminAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check worker management access for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> IsPropertySuperintendentAsync(string userId, int propertyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId, cancellationToken);
            if (property == null)
                return false;

            // Check if user email matches superintendent email
            return property.Superintendent.EmailAddress.Equals(userId, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check superintendent status for user {UserId} and property {PropertyId}", userId, propertyId);
            return false;
        }
    }

    public async Task<bool> IsTenantForRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
            if (request == null)
                return false;

            // Check if user ID matches the tenant ID for the request
            if (int.TryParse(userId, out var tenantId))
            {
                return request.TenantId == tenantId;
            }

            // Check if user email matches tenant email
            var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
            return tenant?.ContactInfo.EmailAddress.Equals(userId, StringComparison.OrdinalIgnoreCase) == true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check tenant ownership for user {UserId} and request {RequestId}", userId, tenantRequestId);
            return false;
        }
    }

    public async Task<bool> IsWorkerAssignedToRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default)
    {
        try
        {
            // In this simplified model, we don't have worker assignments persisted
            // This would be enhanced when worker assignment tracking is implemented
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check worker assignment for user {UserId} and request {RequestId}", userId, tenantRequestId);
            return false;
        }
    }

    private async Task<bool> IsSystemAdminAsync(string userId)
    {
        // Simplified system admin check - in production, check roles from identity provider
        await Task.CompletedTask;
        return userId.EndsWith("@admin.com", StringComparison.OrdinalIgnoreCase);
    }
}