using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Domain.Repositories;
using System.Security.Claims;

namespace RentalRepairs.Infrastructure.Authentication;

/// <summary>
/// Streamlined authentication service implementation
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IPropertyService _propertyService;
    private readonly IWorkerService _workerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IPropertyService propertyService,
        IWorkerService workerService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger)
    {
        _propertyService = propertyService;
        _workerService = workerService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simplified authentication - in production, integrate with proper identity provider
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Email and password are required"
                };
            }

            // For demo purposes, create a basic authenticated user
            var result = new AuthenticationResult
            {
                IsSuccess = true,
                UserId = email,
                Email = email,
                DisplayName = ExtractDisplayName(email),
                Token = GenerateToken(email),
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                Roles = new List<string> { UserRoles.SystemAdmin }
            };

            result.Claims.Add(ClaimTypes.Email, email);
            result.Claims.Add(ClaimTypes.Name, result.DisplayName);
            result.Claims.Add(ClaimTypes.NameIdentifier, email);

            _logger.LogInformation("User {Email} authenticated successfully", email);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication failed for {Email}", email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Authentication failed"
            };
        }
    }

    public async Task<AuthenticationResult> AuthenticateTenantAsync(string email, string propertyCode, string unitNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var property = await _propertyService.GetPropertyByCodeAsync(propertyCode, cancellationToken);
            if (property == null)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid property code"
                };
            }

            var tenant = await _propertyService.GetTenantByPropertyAndUnitAsync(property.Id, unitNumber, cancellationToken);
            if (tenant == null || !tenant.ContactInfo.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid tenant credentials"
                };
            }

            var result = new AuthenticationResult
            {
                IsSuccess = true,
                UserId = tenant.Id.ToString(),
                Email = email,
                DisplayName = $"{tenant.ContactInfo.FirstName} {tenant.ContactInfo.LastName}",
                Token = GenerateToken(email),
                ExpiresAt = DateTime.UtcNow.AddHours(4),
                Roles = new List<string> { UserRoles.Tenant }
            };

            result.Claims.Add(ClaimTypes.Email, email);
            result.Claims.Add(ClaimTypes.Name, result.DisplayName);
            result.Claims.Add(ClaimTypes.NameIdentifier, tenant.Id.ToString());
            result.Claims.Add(CustomClaims.PropertyId, property.Id.ToString());
            result.Claims.Add(CustomClaims.UnitNumber, unitNumber);
            result.Claims.Add(CustomClaims.TenantId, tenant.Id.ToString());

            _logger.LogInformation("Tenant {Email} authenticated for property {PropertyCode} unit {UnitNumber}", 
                email, propertyCode, unitNumber);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tenant authentication failed for {Email}", email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Tenant authentication failed"
            };
        }
    }

    public async Task<AuthenticationResult> AuthenticateWorkerAsync(string email, string specialization, CancellationToken cancellationToken = default)
    {
        try
        {
            var worker = await _workerService.GetWorkerByEmailAsync(email, cancellationToken);
            if (worker == null || !worker.IsActive)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid worker credentials or inactive worker"
                };
            }

            if (!string.IsNullOrEmpty(specialization) && 
                !worker.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase))
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Specialization mismatch"
                };
            }

            var result = new AuthenticationResult
            {
                IsSuccess = true,
                UserId = worker.Id.ToString(),
                Email = email,
                DisplayName = $"{worker.ContactInfo.FirstName} {worker.ContactInfo.LastName}",
                Token = GenerateToken(email),
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                Roles = new List<string> { UserRoles.Worker }
            };

            result.Claims.Add(ClaimTypes.Email, email);
            result.Claims.Add(ClaimTypes.Name, result.DisplayName);
            result.Claims.Add(ClaimTypes.NameIdentifier, worker.Id.ToString());
            result.Claims.Add(CustomClaims.WorkerId, worker.Id.ToString());
            result.Claims.Add(CustomClaims.WorkerSpecialization, worker.Specialization ?? "");
            result.Claims.Add(CustomClaims.IsActive, worker.IsActive.ToString());

            _logger.LogInformation("Worker {Email} authenticated with specialization {Specialization}", 
                email, worker.Specialization);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker authentication failed for {Email}", email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Worker authentication failed"
            };
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simplified token validation - in production, use proper JWT validation
            if (string.IsNullOrEmpty(token))
                return false;

            // Basic token format validation
            await Task.CompletedTask;
            return token.StartsWith("token_") && token.Length > 10;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return false;
        }
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User {UserId} signed out", userId);
            }

            // In a real implementation, you'd invalidate the token/session
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign out failed");
        }
    }

    private string ExtractDisplayName(string email)
    {
        var parts = email.Split('@');
        return parts.Length > 0 ? parts[0] : email;
    }

    private string GenerateToken(string email)
    {
        // Simplified token generation - in production, use proper JWT token generation
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return $"token_{email.Replace("@", "_").Replace(".", "_")}_{timestamp}";
    }
}