namespace RentalRepairs.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for notifications
/// </summary>
public class NotificationSettings
{
    public const string SectionName = "NotificationSettings";

    public string DefaultSenderEmail { get; set; } = "noreply@rentalrepairs.com";
    public string DefaultSenderName { get; set; } = "Rental Repairs System";
    public bool EnableEmailNotifications { get; set; } = true;
    public string NoReplyEmail { get; set; } = "noreply@rentalrepairs.com";
    public EmailProvider EmailProvider { get; set; } = EmailProvider.Mock;
    public bool EnableScheduledNotifications { get; set; } = false;
    public bool EnableBulkNotifications { get; set; } = true;
    public int MaxBulkEmailBatchSize { get; set; } = 100;
    public TimeSpan NotificationRetryDelay { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxNotificationRetries { get; set; } = 3;
}

/// <summary>
/// Email provider options
/// </summary>
public enum EmailProvider
{
    Mock,
    Smtp,
    SendGrid
}

/// <summary>
/// Implementation of INotificationSettings for dependency injection
/// </summary>
public class NotificationSettingsAdapter : RentalRepairs.Application.Common.Interfaces.INotificationSettings
{
    private readonly NotificationSettings _settings;

    public NotificationSettingsAdapter(NotificationSettings settings)
    {
        _settings = settings;
    }

    public string DefaultSenderEmail => _settings.DefaultSenderEmail;
    public string DefaultSenderName => _settings.DefaultSenderName;
    public bool EnableEmailNotifications => _settings.EnableEmailNotifications;
    public string NoReplyEmail => _settings.NoReplyEmail;
}