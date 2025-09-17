namespace RentalRepairs.Application.Interfaces;

/// <summary>
/// Application service for notification operations
/// </summary>
public interface INotifyPartiesService
{
    // Tenant Notifications
    Task NotifyTenantRequestSubmittedAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task NotifyTenantWorkScheduledAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task NotifyTenantWorkCompletedAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task NotifyTenantRequestClosedAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    
    // Superintendent Notifications
    Task NotifySuperintendentNewRequestAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task NotifySuperintendentRequestOverdueAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    Task NotifySuperintendentWorkCompletedAsync(int tenantRequestId, CancellationToken cancellationToken = default);
    
    // Worker Notifications
    Task NotifyWorkerWorkScheduledAsync(int tenantRequestId, string workerEmail, CancellationToken cancellationToken = default);
    Task NotifyWorkerWorkReminderAsync(int tenantRequestId, string workerEmail, CancellationToken cancellationToken = default);
    
    // General Notifications
    Task SendCustomNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default);
    Task NotifyPropertyRegisteredAsync(int propertyId, CancellationToken cancellationToken = default);
    Task NotifyTenantRegisteredAsync(int tenantId, CancellationToken cancellationToken = default);
}