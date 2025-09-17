using RentalRepairs.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace RentalRepairs.Application.Services;

public class NotifyPartiesService : INotifyPartiesService
{
    private readonly ILogger<NotifyPartiesService> _logger;
    private readonly ITenantRequestService _tenantRequestService;
    private readonly IPropertyService _propertyService;
    private readonly IWorkerService _workerService;

    public NotifyPartiesService(
        ILogger<NotifyPartiesService> logger,
        ITenantRequestService tenantRequestService,
        IPropertyService propertyService,
        IWorkerService workerService)
    {
        _logger = logger;
        _tenantRequestService = tenantRequestService;
        _propertyService = propertyService;
        _workerService = workerService;
    }

    public async Task NotifyTenantRequestSubmittedAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        
        var subject = $"Request Submitted: {request.Title}";
        var message = $"Dear {tenant.ContactInfo.FullName},\n\nYour maintenance request has been submitted successfully.\n\nRequest Details:\n- Title: {request.Title}\n- Description: {request.Description}\n- Urgency: {request.UrgencyLevel}\n\nWe will review your request and schedule the necessary work.\n\nThank you.";
        
        await SendCustomNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Tenant notification sent for submitted request {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifyTenantWorkScheduledAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        
        var subject = $"Work Scheduled: {request.Title}";
        var message = $"Dear {tenant.ContactInfo.FullName},\n\nYour maintenance request has been scheduled.\n\nScheduled Date: {request.ScheduledDate:yyyy-MM-dd}\nWork Order: {request.WorkOrderNumber}\nAssigned Worker: {request.AssignedWorkerEmail}\n\nPlease ensure someone is available to provide access.\n\nThank you.";
        
        await SendCustomNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Tenant notification sent for scheduled work {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifyTenantWorkCompletedAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        
        var subject = $"Work Completed: {request.Title}";
        var message = $"Dear {tenant.ContactInfo.FullName},\n\nYour maintenance request has been completed.\n\nCompletion Notes: {request.CompletionNotes}\nCompleted Successfully: {(request.WorkCompletedSuccessfully == true ? "Yes" : "No")}\n\nIf you have any concerns, please contact us.\n\nThank you.";
        
        await SendCustomNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Tenant notification sent for completed work {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifyTenantRequestClosedAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        
        var subject = $"Request Closed: {request.Title}";
        var message = $"Dear {tenant.ContactInfo.FullName},\n\nYour maintenance request has been closed.\n\nClosure Notes: {request.ClosureNotes}\n\nThank you for your patience.\n\nBest regards.";
        
        await SendCustomNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Tenant notification sent for closed request {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifySuperintendentNewRequestAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        var property = await _propertyService.GetPropertyByIdAsync(tenant.PropertyId, cancellationToken);
        
        var subject = $"New Maintenance Request: {property.Name} - Unit {tenant.UnitNumber}";
        var message = $"Dear {property.Superintendent.FullName},\n\nA new maintenance request has been submitted.\n\nProperty: {property.Name} ({property.Code})\nUnit: {tenant.UnitNumber}\nTenant: {tenant.ContactInfo.FullName}\n\nRequest Details:\n- Title: {request.Title}\n- Description: {request.Description}\n- Urgency: {request.UrgencyLevel}\n\nPlease review and schedule the necessary work.\n\nThank you.";
        
        await SendCustomNotificationAsync(property.Superintendent.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Superintendent notification sent for new request {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifySuperintendentRequestOverdueAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        var property = await _propertyService.GetPropertyByIdAsync(tenant.PropertyId, cancellationToken);
        
        var subject = $"OVERDUE Request: {property.Name} - Unit {tenant.UnitNumber}";
        var message = $"Dear {property.Superintendent.FullName},\n\nThe following maintenance request is overdue:\n\nProperty: {property.Name} ({property.Code})\nUnit: {tenant.UnitNumber}\nRequest: {request.Title}\nUrgency: {request.UrgencyLevel}\n\nPlease prioritize scheduling this work.\n\nThank you.";
        
        await SendCustomNotificationAsync(property.Superintendent.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Superintendent notification sent for overdue request {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifySuperintendentWorkCompletedAsync(int tenantRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        var property = await _propertyService.GetPropertyByIdAsync(tenant.PropertyId, cancellationToken);
        
        var subject = $"Work Completed: {property.Name} - Unit {tenant.UnitNumber}";
        var message = $"Dear {property.Superintendent.FullName},\n\nMaintenance work has been completed:\n\nProperty: {property.Name} ({property.Code})\nUnit: {tenant.UnitNumber}\nRequest: {request.Title}\nWorker: {request.AssignedWorkerEmail}\nCompleted Successfully: {(request.WorkCompletedSuccessfully == true ? "Yes" : "No")}\n\nCompletion Notes: {request.CompletionNotes}\n\nThank you.";
        
        await SendCustomNotificationAsync(property.Superintendent.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Superintendent notification sent for completed work {TenantRequestId}", tenantRequestId);
    }

    public async Task NotifyWorkerWorkScheduledAsync(int tenantRequestId, string workerEmail, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var tenant = await _propertyService.GetTenantByIdAsync(request.TenantId, cancellationToken);
        var property = await _propertyService.GetPropertyByIdAsync(tenant.PropertyId, cancellationToken);
        var worker = await _workerService.GetWorkerByEmailAsync(workerEmail, cancellationToken);
        
        var subject = $"Work Assignment: {property.Name} - Unit {tenant.UnitNumber}";
        var message = $"Dear {worker.ContactInfo.FullName},\n\nYou have been assigned to a maintenance request:\n\nProperty: {property.Name} ({property.Code})\nAddress: {property.Address.FullAddress}\nUnit: {tenant.UnitNumber}\nTenant: {tenant.ContactInfo.FullName}\nContact: {tenant.ContactInfo.MobilePhone}\n\nRequest Details:\n- Title: {request.Title}\n- Description: {request.Description}\n- Urgency: {request.UrgencyLevel}\n- Scheduled Date: {request.ScheduledDate:yyyy-MM-dd}\n- Work Order: {request.WorkOrderNumber}\n\nPlease contact the tenant to arrange access.\n\nThank you.";
        
        await SendCustomNotificationAsync(workerEmail, subject, message, cancellationToken);
        
        _logger.LogInformation("Worker notification sent for scheduled work {TenantRequestId} to {WorkerEmail}", tenantRequestId, workerEmail);
    }

    public async Task NotifyWorkerWorkReminderAsync(int tenantRequestId, string workerEmail, CancellationToken cancellationToken = default)
    {
        var request = await _tenantRequestService.GetTenantRequestByIdAsync(tenantRequestId, cancellationToken);
        var worker = await _workerService.GetWorkerByEmailAsync(workerEmail, cancellationToken);
        
        var subject = $"Work Reminder: {request.Title}";
        var message = $"Dear {worker.ContactInfo.FullName},\n\nThis is a reminder about your scheduled maintenance work:\n\nRequest: {request.Title}\nScheduled Date: {request.ScheduledDate:yyyy-MM-dd}\nWork Order: {request.WorkOrderNumber}\n\nPlease ensure you complete this work on schedule.\n\nThank you.";
        
        await SendCustomNotificationAsync(workerEmail, subject, message, cancellationToken);
        
        _logger.LogInformation("Worker reminder sent for {TenantRequestId} to {WorkerEmail}", tenantRequestId, workerEmail);
    }

    public async Task SendCustomNotificationAsync(string recipientEmail, string subject, string message, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would integrate with an email service
        // For now, we'll just log the notification
        _logger.LogInformation("Sending notification to {RecipientEmail}: {Subject}", recipientEmail, subject);
        _logger.LogDebug("Notification message: {Message}", message);
        
        // Simulate async email sending
        await Task.Delay(100, cancellationToken);
    }

    public async Task NotifyPropertyRegisteredAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyService.GetPropertyByIdAsync(propertyId, cancellationToken);
        
        var subject = $"Property Registered: {property.Name}";
        var message = $"Dear {property.Superintendent.FullName},\n\nYour property has been successfully registered in our system:\n\nProperty: {property.Name}\nCode: {property.Code}\nAddress: {property.Address.FullAddress}\nUnits: {string.Join(", ", property.Units)}\n\nYou can now start managing tenant requests for this property.\n\nThank you.";
        
        await SendCustomNotificationAsync(property.Superintendent.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Property registration notification sent for {PropertyId}", propertyId);
    }

    public async Task NotifyTenantRegisteredAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _propertyService.GetTenantByIdAsync(tenantId, cancellationToken);
        var property = await _propertyService.GetPropertyByIdAsync(tenant.PropertyId, cancellationToken);
        
        var subject = $"Welcome to {property.Name}";
        var message = $"Dear {tenant.ContactInfo.FullName},\n\nWelcome to {property.Name}!\n\nYour unit: {tenant.UnitNumber}\nProperty address: {property.Address.FullAddress}\nProperty phone: {property.PhoneNumber}\n\nYou can now submit maintenance requests through our system.\nFor urgent matters, please contact the superintendent at {property.Superintendent.EmailAddress}.\n\nWelcome to your new home!";
        
        await SendCustomNotificationAsync(tenant.ContactInfo.EmailAddress, subject, message, cancellationToken);
        
        _logger.LogInformation("Tenant registration notification sent for {TenantId}", tenantId);
    }
}