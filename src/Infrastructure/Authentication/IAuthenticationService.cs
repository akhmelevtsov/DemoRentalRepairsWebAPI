using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RentalRepairs.Infrastructure.Authentication;

/// <summary>
/// Claims-based authentication service for tenant request management
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> AuthenticateTenantAsync(string email, string propertyCode, string unitNumber, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> AuthenticateWorkerAsync(string email, string specialization, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Authorization service for role-based access control
/// </summary>
public interface IAuthorizationService
{
    Task<bool> CanAccessPropertyAsync(string userId, int propertyId, CancellationToken cancellationToken = default);
    Task<bool> CanAccessTenantRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default);
    Task<bool> CanManageWorkersAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsPropertySuperintendentAsync(string userId, int propertyId, CancellationToken cancellationToken = default);
    Task<bool> IsTenantForRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default);
    Task<bool> IsWorkerAssignedToRequestAsync(string userId, int tenantRequestId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication result model
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Claims { get; set; } = new();
}

/// <summary>
/// User roles for the rental repairs system
/// </summary>
public static class UserRoles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string PropertySuperintendent = "PropertySuperintendent";
    public const string Tenant = "Tenant";
    public const string Worker = "Worker";
}

/// <summary>
/// Custom claims for the rental repairs system
/// </summary>
public static class CustomClaims
{
    public const string PropertyId = "property_id";
    public const string UnitNumber = "unit_number";
    public const string TenantId = "tenant_id";
    public const string WorkerSpecialization = "worker_specialization";
    public const string WorkerId = "worker_id";
    public const string IsActive = "is_active";
}