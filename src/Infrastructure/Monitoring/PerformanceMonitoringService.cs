using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RentalRepairs.Infrastructure.Monitoring;

/// <summary>
/// Performance monitoring service for application operations
/// </summary>
public interface IPerformanceMonitoringService
{
    IDisposable BeginOperation(string operationName, Dictionary<string, object>? parameters = null);
    Task LogPerformanceMetricAsync(string metricName, double value, string unit = "ms", Dictionary<string, object>? tags = null);
    Task LogBusinessMetricAsync(string eventName, Dictionary<string, object> properties);
    Task LogErrorAsync(Exception exception, string operation, Dictionary<string, object>? context = null);
}

/// <summary>
/// Application insights and monitoring service
/// </summary>
public class PerformanceMonitoringService : IPerformanceMonitoringService
{
    private readonly ILogger<PerformanceMonitoringService> _logger;

    public PerformanceMonitoringService(ILogger<PerformanceMonitoringService> logger)
    {
        _logger = logger;
    }

    public IDisposable BeginOperation(string operationName, Dictionary<string, object>? parameters = null)
    {
        return new PerformanceOperation(_logger, operationName, parameters);
    }

    public async Task LogPerformanceMetricAsync(string metricName, double value, string unit = "ms", Dictionary<string, object>? tags = null)
    {
        try
        {
            var logMessage = "Performance Metric: {MetricName} = {Value} {Unit}";
            var logArgs = new List<object> { metricName, value, unit };

            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    logMessage += " | {" + tag.Key + "}";
                    logArgs.Add(tag.Value);
                }
            }

            _logger.LogInformation(logMessage, logArgs.ToArray());
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log performance metric {MetricName}", metricName);
        }
    }

    public async Task LogBusinessMetricAsync(string eventName, Dictionary<string, object> properties)
    {
        try
        {
            var logMessage = "Business Event: {EventName}";
            var logArgs = new List<object> { eventName };

            foreach (var prop in properties)
            {
                logMessage += " | {" + prop.Key + "}";
                logArgs.Add(prop.Value);
            }

            _logger.LogInformation(logMessage, logArgs.ToArray());
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log business event {EventName}", eventName);
        }
    }

    public async Task LogErrorAsync(Exception exception, string operation, Dictionary<string, object>? context = null)
    {
        try
        {
            var logMessage = "Operation Error in {Operation}";
            var logArgs = new List<object> { operation };

            if (context != null)
            {
                foreach (var item in context)
                {
                    logMessage += " | {" + item.Key + "}";
                    logArgs.Add(item.Value);
                }
            }

            _logger.LogError(exception, logMessage, logArgs.ToArray());
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log error for operation {Operation}", operation);
        }
    }
}

/// <summary>
/// Performance operation tracker using IDisposable pattern
/// </summary>
public class PerformanceOperation : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly Dictionary<string, object>? _parameters;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;

    public PerformanceOperation(ILogger logger, string operationName, Dictionary<string, object>? parameters = null)
    {
        _logger = logger;
        _operationName = operationName;
        _parameters = parameters;
        _stopwatch = Stopwatch.StartNew();

        LogOperationStart();
    }

    private void LogOperationStart()
    {
        var logMessage = "Started operation: {OperationName}";
        var logArgs = new List<object> { _operationName };

        if (_parameters != null)
        {
            foreach (var param in _parameters)
            {
                logMessage += " | {" + param.Key + "}";
                logArgs.Add(param.Value);
            }
        }

        _logger.LogDebug(logMessage, logArgs.ToArray());
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _stopwatch.Stop();
        var elapsed = _stopwatch.ElapsedMilliseconds;

        var logMessage = "Completed operation: {OperationName} in {ElapsedMs} ms";
        var logArgs = new List<object> { _operationName, elapsed };

        if (_parameters != null)
        {
            foreach (var param in _parameters)
            {
                logMessage += " | {" + param.Key + "}";
                logArgs.Add(param.Value);
            }
        }

        // Log as warning if operation took too long
        if (elapsed > 5000) // 5 seconds
        {
            _logger.LogWarning(logMessage, logArgs.ToArray());
        }
        else if (elapsed > 1000) // 1 second
        {
            _logger.LogInformation(logMessage, logArgs.ToArray());
        }
        else
        {
            _logger.LogDebug(logMessage, logArgs.ToArray());
        }

        _disposed = true;
    }
}

/// <summary>
/// Business event logger for domain events and important business operations
/// </summary>
public static class BusinessEvents
{
    public const string PropertyRegistered = "PropertyRegistered";
    public const string TenantRegistered = "TenantRegistered";
    public const string TenantRequestSubmitted = "TenantRequestSubmitted";
    public const string TenantRequestScheduled = "TenantRequestScheduled";
    public const string TenantRequestCompleted = "TenantRequestCompleted";
    public const string TenantRequestClosed = "TenantRequestClosed";
    public const string WorkerRegistered = "WorkerRegistered";
    public const string NotificationSent = "NotificationSent";
    public const string AuthenticationAttempt = "AuthenticationAttempt";
    public const string AuthorizationCheck = "AuthorizationCheck";
    public const string CacheOperation = "CacheOperation";
}

/// <summary>
/// Performance metrics constants
/// </summary>
public static class PerformanceMetrics
{
    public const string DatabaseQuery = "database_query_duration";
    public const string CacheHit = "cache_hit_ratio";
    public const string ApiResponse = "api_response_time";
    public const string EmailSend = "email_send_duration";
    public const string NotificationDelivery = "notification_delivery_time";
    public const string AuthenticationDuration = "authentication_duration";
    public const string AuthorizationDuration = "authorization_duration";
}