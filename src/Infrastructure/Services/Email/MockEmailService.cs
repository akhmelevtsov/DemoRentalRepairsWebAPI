using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;

namespace RentalRepairs.Infrastructure.Services.Email;

/// <summary>
/// Mock email service for development and testing
/// </summary>
public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;
    private readonly List<EmailInfo> _sentEmails = new();

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<EmailInfo> SentEmails => _sentEmails.AsReadOnly();
    public EmailInfo? LastSentEmail { get; private set; }

    public async Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // Simulate async operation

        if (emailInfo == null)
        {
            _logger.LogWarning("Attempted to send null email");
            return;
        }

        _sentEmails.Add(emailInfo);
        LastSentEmail = emailInfo;

        _logger.LogInformation("Mock email sent to {RecipientEmail} with subject '{Subject}'", 
            emailInfo.RecipientEmail, emailInfo.Subject);
        _logger.LogDebug("Email body: {Body}", emailInfo.Body);

        // Optionally write to file for development debugging
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var emailLog = $"""
                ==================== MOCK EMAIL ====================
                From: {emailInfo.SenderEmail}
                To: {emailInfo.RecipientEmail}
                CC: {string.Join(", ", emailInfo.CcEmails)}
                BCC: {string.Join(", ", emailInfo.BccEmails)}
                Subject: {emailInfo.Subject}
                Body:
                {emailInfo.Body}
                ====================================================
                """;
            
            _logger.LogDebug("{EmailLog}", emailLog);
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<EmailInfo> emails, CancellationToken cancellationToken = default)
    {
        foreach (var email in emails)
        {
            await SendEmailAsync(email, cancellationToken);
        }
    }

    /// <summary>
    /// Clear sent emails history (useful for testing)
    /// </summary>
    public void ClearHistory()
    {
        _sentEmails.Clear();
        LastSentEmail = null;
    }
}