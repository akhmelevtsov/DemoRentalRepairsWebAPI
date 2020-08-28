using System;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.AzureFunctions
{
    public static class EmailBuilderTriggerFunction
    {
        [FunctionName("EmailBuilderTriggerFunction")]
        public static async Task Run(
            [QueueTrigger(AzureStorageNames.EmailInputDataQueue , Connection = "AzureWebJobsStorage")]string myQueueItem,
            [Queue(AzureStorageNames.EmailOutputDataQueue), StorageAccount("AzureWebJobsStorage")] ICollector<EmailInfo   > storage,
            ILogger log)
        {
            ModelSerializer modelMappers = new ModelSerializer();
           
            log.LogInformation($"C# Queue trigger function processed: ");
            try
            {
                var modelMapper = new ModelMapper();
                if (!string.IsNullOrEmpty(myQueueItem))
                {
   
                    var handler = new NotifyPartiesQueueHandler(new EmailBuilderService( new TemplateDataService() ));
                    var email =  await handler.Handle(myQueueItem,false);
                    if (email != null)
                    {
                        var output = JsonConvert.SerializeObject(email)  ;
                         storage.Add(email);
                    }
                }


            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong with the EmailQueueTrigger {myQueueItem}");
                throw;
            }


        }
    }
}
