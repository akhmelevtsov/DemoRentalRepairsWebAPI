using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Notifications.Models;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Microsoft.Azure.Functions.Worker;
using System;

namespace Demo.RentalRepairs.AzureFunctions
{
    public class SendEmailFunction
    {
        private readonly ILogger<SendEmailFunction> _logger;

        public SendEmailFunction(ILogger<SendEmailFunction> logger)
        {
            _logger = logger;
        }

        [Function("SendEmailFunction")]
        [SendGridOutput(ApiKey = "CustomSendGridKeyAppSettingName")]
        public SendGridMessage Run(
            [QueueTrigger(AzureStorageNames.EmailOutputDataQueue, Connection = "AzureWebJobsStorage")] EmailInfo emailInfo)
        {
            var message = new SendGridMessage();
            message.AddTo(emailInfo.RecipientEmail);
            message.AddContent("text/html", emailInfo.Body);
            message.SetFrom(new EmailAddress(emailInfo.SenderEmail));
            message.SetSubject(emailInfo.Subject);
            
            try
            {
                _logger.LogInformation($"Preparing to send email to {emailInfo.RecipientEmail}");
                
                // Create email info model for logging/tracking
                var emailInfoModel = new EmailInfoModel()
                {
                    PartitionKey = "Emails",
                    RowKey = Guid.NewGuid().ToString(),
                    Body = emailInfo.Body,
                    RecipientEmail = emailInfo.RecipientEmail,
                    SenderEmail = emailInfo.SenderEmail,
                    Subject = emailInfo.Subject
                };

                // Note: Table output binding is not available in isolated worker model
                // You would need to use Table API directly or implement a separate function
                // For now, we'll just log the email info
                _logger.LogInformation($"Email info to be stored: {System.Text.Json.JsonSerializer.Serialize(emailInfoModel)}");

                _logger.LogInformation($"SendEmailFunction processed!");
                
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong with sending email {message}");
                throw;
            }
        }
    }
}
