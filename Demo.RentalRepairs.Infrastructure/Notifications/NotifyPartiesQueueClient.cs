using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Infrastructure.Notifications.Interfaces;
using Demo.RentalRepairs.Infrastructure.Notifications.Models;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Notifications
{
    public class NotifyPartiesQueueClient : BaseNotifyPartiesService,  INotifyPartiesService
    {
        private readonly IQueueClient _queueClient;
        private readonly ModelSerializer _modelSerializer = new ModelSerializer();
        private readonly ModelMapper _modelMapper = new ModelMapper();

        public NotifyPartiesQueueClient(IQueueClient queueClient,  IPropertyRepository propertyRepository) :base (propertyRepository )
        {
            _queueClient = queueClient;
        }
        public async Task CreateAndSendEmailAsync(TenantRequest tenantRequest)
        {
            //var connectionString =
            //    "DefaultEndpointsProtocol=https;AccountName=demorrstorageaccount;AccountKey=NXluNSsP4tm+NViZ65rN6eM4V6Eniko9b7NON3M5xZGaDmGMV0LSIsM9CUVdFe+2n5L8vyl4DtdaVZz+JMJ3iA==;EndpointSuffix=core.windows.net";

            var model = new NotificationQueueModel()
            {
                TenantRequestModel = _modelMapper.CopyFrom( tenantRequest),
               
            };
            var det = base.GetWorkerDetails(tenantRequest);
            model.WorkerModel = det == null ? null : _modelMapper.CopyFrom( det);

            var message = _modelSerializer.Serialize(model);

            //var connectionString = "UseDevelopmentStorage=true";


            //// Instantiate a QueueClient which will be used to create and manipulate the queue
            //QueueClient queueClient = new QueueClient(connectionString, "service-requests-queue");// "ServiceRequestsQueue");
            //// Create the queue
            //await queueClient.CreateIfNotExistsAsync();
            //// Send a message to the queue

            //await queueClient.SendMessageAsync(message); //Base64Encode(message));
            await _queueClient.SendMessage(message);

            // Async receive the message
            //QueueMessage[] retrievedMessage = await queueClient.ReceiveMessagesAsync();

            //var tr = _modelMappers.CopyFrom(retrievedMessage[0].MessageText);

        }

        public Task CreateAndSendEmailAsync(TenantRequest tenantRequest, Worker worker)
        {
            throw new NotImplementedException();
        }

        public Task<EmailInfo> CreateTenantRequestEmailAsync(TenantRequest tenantRequest, Worker worker)
        {
            throw new NotImplementedException();
        }


        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
