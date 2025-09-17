namespace RentalRepairs.Application.Common.Interfaces;

/// <summary>
/// Configuration for email notifications
/// </summary>
public interface INotificationSettings
{
    string DefaultSenderEmail { get; }
    string DefaultSenderName { get; }
    bool EnableEmailNotifications { get; }
    string NoReplyEmail { get; }
}

/// <summary>
/// Enhanced notification service interface
/// </summary>
public interface INotificationService
{
    // Core email functionality
    Task SendEmailNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default);
    Task SendTemplatedEmailAsync(string recipientEmail, string templateId, object templateData, CancellationToken cancellationToken = default);
    
    // Bulk notifications
    Task SendBulkNotificationAsync(IEnumerable<string> recipientEmails, string subject, string message, CancellationToken cancellationToken = default);
    
    // Business domain notifications
    Task NotifyTenantAsync(int tenantId, string subject, string message, CancellationToken cancellationToken = default);
    Task NotifyPropertySuperintendentAsync(int propertyId, string subject, string message, CancellationToken cancellationToken = default);
    Task NotifyWorkerAsync(string workerEmail, string subject, string message, CancellationToken cancellationToken = default);
    
    // Advanced notification features
    Task ScheduleNotificationAsync(string recipientEmail, string subject, string message, DateTime scheduledTime, CancellationToken cancellationToken = default);
    Task SendUrgentNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default);
}