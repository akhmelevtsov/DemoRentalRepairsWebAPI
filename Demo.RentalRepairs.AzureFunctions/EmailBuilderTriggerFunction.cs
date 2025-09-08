using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.RentalRepairs.AzureFunctions
{
    public class EmailBuilderTriggerFunction
    {
        private readonly ILogger<EmailBuilderTriggerFunction> _logger;

        public EmailBuilderTriggerFunction(ILogger<EmailBuilderTriggerFunction> logger)
        {
            _logger = logger;
        }

        [Function("EmailBuilderTriggerFunction")]
        [QueueOutput(AzureStorageNames.EmailOutputDataQueue, Connection = "AzureWebJobsStorage")]
        public async Task<EmailInfo> Run(
            [QueueTrigger(AzureStorageNames.EmailInputDataQueue, Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            ModelSerializer modelMappers = new ModelSerializer();
           
            _logger.LogInformation($"C# Queue trigger function processed: ");
            try
            {
                var modelMapper = new ModelMapper();
                if (!string.IsNullOrEmpty(myQueueItem))
                {
   
                    var handler = new NotifyPartiesQueueHandler(new EmailBuilderService( new TemplateDataService() ));
                    var email =  await handler.Handle(myQueueItem,false);
                    if (email != null)
                    {
                        return email;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong with the EmailQueueTrigger {myQueueItem}");
                throw;
            }
        }
    }
}
