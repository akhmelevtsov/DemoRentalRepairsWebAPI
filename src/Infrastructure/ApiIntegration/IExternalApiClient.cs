using System.Text.Json;

namespace RentalRepairs.Infrastructure.ApiIntegration;

/// <summary>
/// External API client interface for worker scheduling integration
/// </summary>
public interface IExternalApiClient
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class;
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : class where TResponse : class;
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : class where TResponse : class;
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}

/// <summary>
/// Worker scheduling API client for external integration
/// </summary>
public interface IWorkerSchedulingApiClient
{
    Task<WorkerAvailabilityResponse?> GetWorkerAvailabilityAsync(string workerEmail, DateTime date, CancellationToken cancellationToken = default);
    Task<ScheduleWorkResponse?> ScheduleWorkAsync(ScheduleWorkRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelScheduledWorkAsync(string workOrderId, CancellationToken cancellationToken = default);
    Task<WorkerScheduleResponse?> GetWorkerScheduleAsync(string workerEmail, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

/// <summary>
/// API request/response models for worker scheduling
/// </summary>
public class WorkerAvailabilityResponse
{
    public string WorkerEmail { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public List<TimeSlot> AvailableSlots { get; set; } = new();
    public string? Reason { get; set; }
}

public class ScheduleWorkRequest
{
    public string WorkerEmail { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public string WorkType { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UrgencyLevel { get; set; } = string.Empty;
}

public class ScheduleWorkResponse
{
    public bool IsSuccess { get; set; }
    public string WorkOrderId { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class WorkerScheduleResponse
{
    public string WorkerEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ScheduledWork> ScheduledWork { get; set; } = new();
}

public class ScheduledWork
{
    public string WorkOrderId { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public TimeSpan Duration { get; set; }
    public string WorkType { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class TimeSlot
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
}