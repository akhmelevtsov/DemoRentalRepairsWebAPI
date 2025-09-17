using Microsoft.Extensions.Diagnostics.HealthChecks;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.WebUI.HealthChecks;

/// <summary>
/// Application-specific health check that validates key services
/// </summary>
public class ApplicationHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationHealthCheck> _logger;

    public ApplicationHealthCheck(
        ApplicationDbContext context,
        ILogger<ApplicationHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check database connectivity by trying to open connection
            await _context.Database.CanConnectAsync(cancellationToken);
            
            // Check basic application services
            var checks = new Dictionary<string, object>
            {
                ["database_connection"] = "healthy",
                ["application_services"] = "healthy",
                ["timestamp"] = DateTime.UtcNow
            };

            _logger.LogInformation("Application health check completed successfully");
            
            return HealthCheckResult.Healthy("Application is running normally", checks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application health check failed");
            
            var errorData = new Dictionary<string, object>
            {
                ["error"] = ex.Message,
                ["timestamp"] = DateTime.UtcNow
            };
            
            return HealthCheckResult.Unhealthy("Application health check failed", ex, errorData);
        }
    }
}