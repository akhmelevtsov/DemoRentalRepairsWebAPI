namespace RentalRepairs.Infrastructure.Configuration;

/// <summary>
/// External services configuration
/// </summary>
public class ExternalServicesSettings
{
    public const string SectionName = "ExternalServices";

    public SmtpSettings Smtp { get; set; } = new();
    public SendGridSettings SendGrid { get; set; } = new();
    public ApiIntegrationSettings ApiIntegrations { get; set; } = new();
}

/// <summary>
/// SMTP configuration settings
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public bool EnableAuthentication { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// SendGrid configuration settings
/// </summary>
public class SendGridSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public bool EnableClickTracking { get; set; } = false;
    public bool EnableOpenTracking { get; set; } = false;
    public string WebhookUrl { get; set; } = string.Empty;
}

/// <summary>
/// External API integration settings
/// </summary>
public class ApiIntegrationSettings
{
    public bool EnableWorkerSchedulingApi { get; set; } = false;
    public string WorkerSchedulingApiUrl { get; set; } = string.Empty;
    public string WorkerSchedulingApiKey { get; set; } = string.Empty;
    public int ApiTimeoutSeconds { get; set; } = 30;
    public int MaxRetryAttempts { get; set; } = 3;
}