using System;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Demo.RentalRepairs.AzureFunctions
{
   
    public static class SendEmailFunction
    {
        [FunctionName("SendEmailFunction")]
        public static async Task Run(
            [QueueTrigger(AzureStorageNames.EmailOutputDataQueue, Connection = "AzureWebJobsStorage")]EmailInfo emailInfo,
            [SendGrid(ApiKey = "CustomSendGridKeyAppSettingName")] IAsyncCollector<SendGridMessage> messageCollector,
            [Table(AzureStorageNames.SentEmailsTable), StorageAccount("AzureWebJobsStorage")]IAsyncCollector<EmailInfoModel> outputTable,
            ILogger log)
        {

            var message = new SendGridMessage();
            message.AddTo(emailInfo.RecipientEmail);
            message.AddContent("text/html", emailInfo.Body);
            message.SetFrom(new EmailAddress(emailInfo.SenderEmail));
            message.SetSubject(emailInfo.Subject);
            try
            {
                await messageCollector.AddAsync(message);

            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong with sending email {message}");
                throw;
            }

            var emailInfoModel = new EmailInfoModel()
            {
                PartitionKey = "Emails",
                RowKey = Guid.NewGuid().ToString(),
                Body = emailInfo.Body,
                RecipientEmail = emailInfo.RecipientEmail,
                SenderEmail = emailInfo.SenderEmail,
                Subject = emailInfo.Subject
            };

           
            try
            {
                await outputTable.AddAsync(emailInfoModel);

            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong with storing email {emailInfoModel}");
              
            }

            log.LogInformation($"SendEmailFunction processed ! ");


        }
    }
}
