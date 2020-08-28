using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Demo.RentalRepairs.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.Options;

namespace Demo.RentalRepairs.Infrastructure.Notifications
{
    public class AzureStorageQueueClient : IQueueClient
    {
        private readonly QueueClient _queueClient;// "ServiceRequestsQueue");
        public AzureStorageQueueClient(IOptions<NotificationQueueSettings> settings)
        {
            var settings1 = settings.Value;
            //var connectionString = "UseDevelopmentStorage=true";

            // Instantiate a QueueClient which will be used to create and manipulate the queue

            _queueClient = new QueueClient(settings1.ConnectionString , AzureStorageNames.EmailInputDataQueue  );
            // Create the queue
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessage(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }
    }
}
