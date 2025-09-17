using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RentalRepairs.Infrastructure.Services.Email;

/// <summary>
/// SendGrid email service implementation
/// </summary>
public class SendGridEmailService : IEmailService
{
    private readonly SendGridEmailOptions _options;
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        SendGridEmailOptions options, 
        ISendGridClient sendGridClient, 
        ILogger<SendGridEmailService> logger)
    {
        _options = options;
        _sendGridClient = sendGridClient;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var from = new EmailAddress(
                string.IsNullOrEmpty(emailInfo.SenderEmail) ? _options.DefaultSenderEmail : emailInfo.SenderEmail,
                _options.DefaultSenderName);

            var to = new EmailAddress(emailInfo.RecipientEmail);

            var msg = MailHelper.CreateSingleEmail(
                from, 
                to, 
                emailInfo.Subject, 
                emailInfo.IsBodyHtml ? null : emailInfo.Body, 
                emailInfo.IsBodyHtml ? emailInfo.Body : null);

            // Add CC recipients
            if (emailInfo.CcEmails.Any())
            {
                var ccList = emailInfo.CcEmails.Select(email => new EmailAddress(email)).ToList();
                msg.AddCcs(ccList);
            }

            // Add BCC recipients
            if (emailInfo.BccEmails.Any())
            {
                var bccList = emailInfo.BccEmails.Select(email => new EmailAddress(email)).ToList();
                msg.AddBccs(bccList);
            }

            // Add custom headers
            foreach (var header in emailInfo.Headers)
            {
                msg.AddHeader(header.Key, header.Value);
            }

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SendGrid email sent successfully to {RecipientEmail} with subject '{Subject}'", 
                    emailInfo.RecipientEmail, emailInfo.Subject);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid email failed with status {StatusCode}: {ResponseBody}", 
                    response.StatusCode, responseBody);
                throw new InvalidOperationException($"SendGrid email failed with status {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SendGrid email to {RecipientEmail} with subject '{Subject}'", 
                emailInfo.RecipientEmail, emailInfo.Subject);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<EmailInfo> emails, CancellationToken cancellationToken = default)
    {
        // SendGrid supports bulk sending, but for simplicity we'll send individually
        // In a production scenario, you'd want to use SendGrid's bulk email features
        var tasks = emails.Select(email => SendEmailAsync(email, cancellationToken));
        await Task.WhenAll(tasks);
    }
}

/// <summary>
/// Configuration options for SendGrid email service
/// </summary>
public class SendGridEmailOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultSenderEmail { get; set; } = string.Empty;
    public string DefaultSenderName { get; set; } = "Rental Repairs System";
}