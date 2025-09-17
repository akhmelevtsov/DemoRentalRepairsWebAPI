using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RentalRepairs.Infrastructure.Services.Email;

/// <summary>
/// SMTP Email Service implementation
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpEmailOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(SmtpEmailOptions options, ILogger<SmtpEmailService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = new MailMessage();
            
            // Set sender
            message.From = new MailAddress(
                string.IsNullOrEmpty(emailInfo.SenderEmail) ? _options.DefaultSenderEmail : emailInfo.SenderEmail,
                _options.DefaultSenderName);

            // Set recipient
            message.To.Add(new MailAddress(emailInfo.RecipientEmail));

            // Add CC recipients
            foreach (var ccEmail in emailInfo.CcEmails)
            {
                message.CC.Add(new MailAddress(ccEmail));
            }

            // Add BCC recipients
            foreach (var bccEmail in emailInfo.BccEmails)
            {
                message.Bcc.Add(new MailAddress(bccEmail));
            }

            // Set content
            message.Subject = emailInfo.Subject;
            message.Body = emailInfo.Body;
            message.IsBodyHtml = emailInfo.IsBodyHtml;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            // Add custom headers
            foreach (var header in emailInfo.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            using var client = new SmtpClient(_options.Host, _options.Port);
            
            if (_options.EnableAuthentication)
            {
                client.Credentials = new NetworkCredential(_options.Username, _options.Password);
            }
            
            client.EnableSsl = _options.EnableSsl;

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent successfully to {RecipientEmail} with subject '{Subject}'", 
                emailInfo.RecipientEmail, emailInfo.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail} with subject '{Subject}'", 
                emailInfo.RecipientEmail, emailInfo.Subject);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<EmailInfo> emails, CancellationToken cancellationToken = default)
    {
        var tasks = emails.Select(email => SendEmailAsync(email, cancellationToken));
        await Task.WhenAll(tasks);
    }
}

/// <summary>
/// Configuration options for SMTP email service
/// </summary>
public class SmtpEmailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public bool EnableAuthentication { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DefaultSenderEmail { get; set; } = string.Empty;
    public string DefaultSenderName { get; set; } = "Rental Repairs System";
}