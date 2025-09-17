using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Interfaces;

namespace RentalRepairs.Infrastructure.Services.Notifications;

/// <summary>
/// Comprehensive notification service implementation
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly INotificationSettings _notificationSettings;
    private readonly IPropertyService _propertyService;
    private readonly ITenantRequestService _tenantRequestService;
    private readonly IWorkerService _workerService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        INotificationSettings notificationSettings,
        IPropertyService propertyService,
        ITenantRequestService tenantRequestService,
        IWorkerService workerService,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _notificationSettings = notificationSettings;
        _propertyService = propertyService;
        _tenantRequestService = tenantRequestService;
        _workerService = workerService;
        _logger = logger;
    }

    public async Task SendEmailNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        if (!_notificationSettings.EnableEmailNotifications)
        {
            _logger.LogInformation("Email notifications are disabled. Skipping email to {RecipientEmail}", recipientEmail);
            return;
        }

        var emailInfo = new EmailInfo
        {
            SenderEmail = _notificationSettings.DefaultSenderEmail,
            RecipientEmail = recipientEmail,
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        await _emailService.SendEmailAsync(emailInfo, cancellationToken);
        _logger.LogInformation("Email notification sent to {RecipientEmail} with subject '{Subject}'", recipientEmail, subject);
    }

    public async Task SendTemplatedEmailAsync(string recipientEmail, string templateId, object templateData, CancellationToken cancellationToken = default)
    {
        // For now, this is a simple implementation
        // In a full implementation, you'd have a template engine
        var subject = $"Notification - {templateId}";
        var message = $"Template: {templateId}, Data: {System.Text.Json.JsonSerializer.Serialize(templateData)}";
        
        await SendEmailNotificationAsync(recipientEmail, subject, message, cancellationToken);
    }

    public async Task SendBulkNotificationAsync(IEnumerable<string> recipientEmails, string subject, string message, CancellationToken cancellationToken = default)
    {
        if (!_notificationSettings.EnableEmailNotifications)
        {
            _logger.LogInformation("Email notifications are disabled. Skipping bulk email");
            return;
        }

        var emails = recipientEmails.Select(email => new EmailInfo
        {
            SenderEmail = _notificationSettings.DefaultSenderEmail,
            RecipientEmail = email,
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        });

        await _emailService.SendBulkEmailAsync(emails, cancellationToken);
        _logger.LogInformation("Bulk email notification sent to {RecipientCount} recipients with subject '{Subject}'", 
            recipientEmails.Count(), subject);
    }

    public async Task NotifyTenantAsync(int tenantId, string subject, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenant = await _propertyService.GetTenantByIdAsync(tenantId, cancellationToken);
            await SendEmailNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
            _logger.LogInformation("Tenant notification sent to tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task NotifyPropertySuperintendentAsync(int propertyId, string subject, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId, cancellationToken);
            await SendEmailNotificationAsync(property.Superintendent.EmailAddress, subject, message, cancellationToken);
            _logger.LogInformation("Superintendent notification sent for property {PropertyId}", propertyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to superintendent for property {PropertyId}", propertyId);
            throw;
        }
    }

    public async Task NotifyWorkerAsync(string workerEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            await SendEmailNotificationAsync(workerEmail, subject, message, cancellationToken);
            _logger.LogInformation("Worker notification sent to {WorkerEmail}", workerEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to worker {WorkerEmail}", workerEmail);
            throw;
        }
    }

    public async Task ScheduleNotificationAsync(string recipientEmail, string subject, string message, DateTime scheduledTime, CancellationToken cancellationToken = default)
    {
        // For now, we'll just send immediately
        // In a production system, you'd integrate with a job scheduler like Hangfire or Quartz
        if (scheduledTime <= DateTime.UtcNow)
        {
            await SendEmailNotificationAsync(recipientEmail, subject, message, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Scheduled notification for {RecipientEmail} at {ScheduledTime} - queuing for future delivery", 
                recipientEmail, scheduledTime);
            
            // TODO: Integrate with job scheduler
            // For now, just log the scheduled notification
            await Task.CompletedTask;
        }
    }

    public async Task SendUrgentNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        // Mark as urgent and send immediately
        var urgentSubject = $"URGENT: {subject}";
        var urgentMessage = $"?? URGENT NOTIFICATION ??\n\n{message}";
        
        await SendEmailNotificationAsync(recipientEmail, urgentSubject, urgentMessage, cancellationToken);
        _logger.LogWarning("Urgent notification sent to {RecipientEmail} with subject '{Subject}'", recipientEmail, urgentSubject);
    }
}