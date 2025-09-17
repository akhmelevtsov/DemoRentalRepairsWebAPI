namespace RentalRepairs.Application.Common.Interfaces;

/// <summary>
/// Email information for sending emails
/// </summary>
public class EmailInfo
{
    public string SenderEmail { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsBodyHtml { get; set; } = true;
    public List<string> CcEmails { get; set; } = new();
    public List<string> BccEmails { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
}

/// <summary>
/// Email service interface for sending emails
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(IEnumerable<EmailInfo> emails, CancellationToken cancellationToken = default);
}