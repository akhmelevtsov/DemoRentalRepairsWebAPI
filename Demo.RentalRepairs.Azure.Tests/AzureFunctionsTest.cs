using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Notifications.Interfaces;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Tests.Integration.Shared;
using mailslurp.Api;
using mailslurp.Client;
using mailslurp.Model;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Azure.Tests
{
    [TestClass]
    public class AzureFunctionsTest
    {
        private static TestFixture _fixture;
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _fixture = new TestFixture();
        }
        [ClassCleanup]
        public static void CloseBrowser()
        {
            _fixture.Dispose();
        }
        [TestMethod]
        public async Task AzureFunctionsHappyPathTestWithInMemoryRepoAndServices()
        {
            var services = new ServiceCollection();
            services.Configure<NotificationQueueSettings>
                (options => options.ConnectionString = "UseDevelopmentStorage=true");

            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<IUserAuthorizationService , UserAuthorizationMockService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddSingleton<IQueueClient, AzureStorageQueueClient>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesQueueClient>();
            services.AddTransient<NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();

            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>();


            var superInbox = CreateInbox();

            var tenantInbox = CreateInbox();
            var workerInbox = CreateInbox();

            var worker1Inbox = CreateInbox();


            await SharedTests.TestHappyPath(repo, propertyService, authService, superInbox.EmailAddress, tenantInbox.EmailAddress , workerInbox.EmailAddress , worker1Inbox.EmailAddress );

            await Task.Delay(25000);

            var table =_fixture.TableClient.ListTables().FirstOrDefault(x => x.Name == AzureStorageNames.SentEmailsTable);
            Assert.IsNotNull(table);
          
            TableQuery<EmailRecordHeader> query = new TableQuery<EmailRecordHeader>();
            int count = 0;
            foreach (EmailRecordHeader entity in table.ExecuteQuery(query))
            {
                Assert.IsNotNull(entity.Properties);
                Assert.AreEqual(4, entity.Properties.Count());
                count++;
            }

            Assert.AreEqual(10, count);
            // super -3, tenant - 4, worker - 2, worker1- 1

            var timeout = 30000L; // max milliseconds to wait
            AssertInboxEmails(superInbox, 3, timeout);
            AssertInboxEmails(tenantInbox , 4, timeout);
            AssertInboxEmails(workerInbox , 2, timeout);
            AssertInboxEmails(worker1Inbox, 1, timeout);
        }

        private static void AssertInboxEmails(Inbox inbox,int num,  long timeout)
        {
            var waitForInstance = new WaitForControllerApi(_fixture.Config);
            var list = waitForInstance.WaitForEmailCount(num, inbox.Id, timeout, true);
            Assert.IsNotNull(list);
            Assert.AreEqual(num, list.Count);
            foreach (var e in list)
            {
                Assert.IsNotNull(e.Subject); 

            }
           
        }

        private static Inbox CreateInbox()
        {
            var inbox = _fixture.ApiInstance.CreateInbox();

            Assert.IsNotNull(inbox);
            Assert.IsTrue(inbox.EmailAddress.Contains("@mailslurp.com"));
            return inbox;
        }

       



        public class EmailRecordHeader :ITableEntity 
        {
            private IDictionary<string, EntityProperty> _properties;

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                _properties = properties;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public IDictionary<string, EntityProperty> Properties => _properties;
        }
    }
}
