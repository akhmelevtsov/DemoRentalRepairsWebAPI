using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentalRepairs.Infrastructure.Configuration;
using RentalRepairs.Infrastructure.Monitoring;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RentalRepairs.Infrastructure.ApiIntegration;

/// <summary>
/// HTTP-based external API client implementation
/// </summary>
public class HttpExternalApiClient : IExternalApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalServicesSettings _settings;
    private readonly IPerformanceMonitoringService _performanceMonitoring;
    private readonly ILogger<HttpExternalApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpExternalApiClient(
        HttpClient httpClient,
        IOptions<ExternalServicesSettings> settings,
        IPerformanceMonitoringService performanceMonitoring,
        ILogger<HttpExternalApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _performanceMonitoring = performanceMonitoring;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrEmpty(_settings.ApiIntegrations.WorkerSchedulingApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _settings.ApiIntegrations.WorkerSchedulingApiKey);
        }

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "RentalRepairs-System/1.0");

        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.ApiIntegrations.ApiTimeoutSeconds);
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class
    {
        using var operation = _performanceMonitoring.BeginOperation("ExternalApi_GET", new Dictionary<string, object>
        {
            ["endpoint"] = endpoint,
            ["responseType"] = typeof(T).Name
        });

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            await _performanceMonitoring.LogErrorAsync(ex, "ExternalApi_GET", new Dictionary<string, object>
            {
                ["endpoint"] = endpoint
            });
            throw;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : class where TResponse : class
    {
        using var operation = _performanceMonitoring.BeginOperation("ExternalApi_POST", new Dictionary<string, object>
        {
            ["endpoint"] = endpoint,
            ["requestType"] = typeof(TRequest).Name,
            ["responseType"] = typeof(TResponse).Name
        });

        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            return await ProcessResponse<TResponse>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            await _performanceMonitoring.LogErrorAsync(ex, "ExternalApi_POST", new Dictionary<string, object>
            {
                ["endpoint"] = endpoint
            });
            throw;
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : class where TResponse : class
    {
        using var operation = _performanceMonitoring.BeginOperation("ExternalApi_PUT", new Dictionary<string, object>
        {
            ["endpoint"] = endpoint,
            ["requestType"] = typeof(TRequest).Name,
            ["responseType"] = typeof(TResponse).Name
        });

        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            return await ProcessResponse<TResponse>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            await _performanceMonitoring.LogErrorAsync(ex, "ExternalApi_PUT", new Dictionary<string, object>
            {
                ["endpoint"] = endpoint
            });
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        using var operation = _performanceMonitoring.BeginOperation("ExternalApi_DELETE", new Dictionary<string, object>
        {
            ["endpoint"] = endpoint
        });

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            await _performanceMonitoring.LogErrorAsync(ex, "ExternalApi_DELETE", new Dictionary<string, object>
            {
                ["endpoint"] = endpoint
            });
            return false;
        }
    }

    private async Task<T?> ProcessResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken) where T : class
    {
        try
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("External API request failed with status {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrEmpty(responseContent))
                return null;

            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize API response to {Type}", typeof(T).Name);
            return null;
        }
    }
}

/// <summary>
/// Worker scheduling API client implementation
/// </summary>
public class WorkerSchedulingApiClient : IWorkerSchedulingApiClient
{
    private readonly IExternalApiClient _apiClient;
    private readonly ExternalServicesSettings _settings;
    private readonly ILogger<WorkerSchedulingApiClient> _logger;

    public WorkerSchedulingApiClient(
        IExternalApiClient apiClient,
        IOptions<ExternalServicesSettings> settings,
        ILogger<WorkerSchedulingApiClient> logger)
    {
        _apiClient = apiClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<WorkerAvailabilityResponse?> GetWorkerAvailabilityAsync(string workerEmail, DateTime date, CancellationToken cancellationToken = default)
    {
        if (!_settings.ApiIntegrations.EnableWorkerSchedulingApi)
        {
            _logger.LogDebug("Worker scheduling API is disabled, returning mock availability");
            return CreateMockAvailability(workerEmail, date);
        }

        var endpoint = $"workers/{Uri.EscapeDataString(workerEmail)}/availability?date={date:yyyy-MM-dd}";
        return await _apiClient.GetAsync<WorkerAvailabilityResponse>(endpoint, cancellationToken);
    }

    public async Task<ScheduleWorkResponse?> ScheduleWorkAsync(ScheduleWorkRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.ApiIntegrations.EnableWorkerSchedulingApi)
        {
            _logger.LogDebug("Worker scheduling API is disabled, returning mock schedule response");
            return CreateMockScheduleResponse(request);
        }

        return await _apiClient.PostAsync<ScheduleWorkRequest, ScheduleWorkResponse>("work-orders", request, cancellationToken);
    }

    public async Task<bool> CancelScheduledWorkAsync(string workOrderId, CancellationToken cancellationToken = default)
    {
        if (!_settings.ApiIntegrations.EnableWorkerSchedulingApi)
        {
            _logger.LogDebug("Worker scheduling API is disabled, returning mock cancellation success");
            return true;
        }

        return await _apiClient.DeleteAsync($"work-orders/{Uri.EscapeDataString(workOrderId)}", cancellationToken);
    }

    public async Task<WorkerScheduleResponse?> GetWorkerScheduleAsync(string workerEmail, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (!_settings.ApiIntegrations.EnableWorkerSchedulingApi)
        {
            _logger.LogDebug("Worker scheduling API is disabled, returning mock schedule");
            return CreateMockSchedule(workerEmail, startDate, endDate);
        }

        var endpoint = $"workers/{Uri.EscapeDataString(workerEmail)}/schedule?start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}";
        return await _apiClient.GetAsync<WorkerScheduleResponse>(endpoint, cancellationToken);
    }

    private WorkerAvailabilityResponse CreateMockAvailability(string workerEmail, DateTime date)
    {
        return new WorkerAvailabilityResponse
        {
            WorkerEmail = workerEmail,
            Date = date,
            IsAvailable = true,
            AvailableSlots = new List<TimeSlot>
            {
                new() { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(12, 0, 0), IsAvailable = true },
                new() { StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsAvailable = true }
            }
        };
    }

    private ScheduleWorkResponse CreateMockScheduleResponse(ScheduleWorkRequest request)
    {
        return new ScheduleWorkResponse
        {
            IsSuccess = true,
            WorkOrderId = request.WorkOrderId,
            ScheduledDate = request.ScheduledDate,
            ConfirmationNumber = $"CONF-{DateTime.UtcNow.Ticks}"
        };
    }

    private WorkerScheduleResponse CreateMockSchedule(string workerEmail, DateTime startDate, DateTime endDate)
    {
        return new WorkerScheduleResponse
        {
            WorkerEmail = workerEmail,
            StartDate = startDate,
            EndDate = endDate,
            ScheduledWork = new List<ScheduledWork>()
        };
    }
}