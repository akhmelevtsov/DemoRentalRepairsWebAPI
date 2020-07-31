using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Core.Tests.Integration
{

    [TestClass]
    public class TenantRequestTests
    {
        [TestMethod]
        public void DomainAllPathsTest()
        {
            //var repo = new PropertyRepositoryMock();
            //var ntfService = new NotifyPartiesServiceMock();

            PropertyDomainService.DateTimeProvider = new DateTimeProviderMock();

            var worker = new Worker(new PersonContactInfo()
            {
                LastName = "Douglas",
                FirstName = "Jordan",
                EmailAddress = "worker@Workers.com",
                MobilePhone = "647-222-2222"
            });

            var worker1 = new Worker(new PersonContactInfo()
            {

                LastName = "Lee",
                FirstName = "Pete",
                EmailAddress = "worker2@Workers.com",
                MobilePhone = "647-222-2223"

            });

            var prop = new Property(new AddPropertyCommand("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" }, "noreply@moonlightapartments.com"));

            Assert.AreEqual(DateTimeProviderMock.CurrentDate, prop.DateCreated);

            //prop.RegisterWorker


            var tenant = prop.RegisterTenant(
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com",
                    FirstName = "John",
                    LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );

            Assert.AreEqual(DateTimeProviderMock.CurrentDate, tenant.DateCreated);

            var tenantRequest = tenant.AddRequest(
                new RegisterTenantRequestCommand()
                {
                    Title =  "Power plug in kitchen",
                    Description = "The plug is broken"

                });
            Assert.AreEqual(DateTimeProviderMock.CurrentDate, tenantRequest.DateCreated);
            Assert.IsNotNull(tenantRequest.Id);
            Assert.AreEqual("1", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
            
            tenantRequest = tenantRequest.ExecuteCommand(  new ScheduleServiceWorkCommand(worker.PersonContactInfo.EmailAddress , DateTime.Today, 1));
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand (new ReportServiceWorkCommand() { Notes = "All done", Success =true });
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            tenantRequest.ExecuteCommand( new CloseTenantRequestCommand());
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            tenantRequest = tenant.AddRequest(new RegisterTenantRequestCommand()
            {
                Title = "Kitchen desk replace",
                Description ="Kitchen desk is in awful condition"

            });
            Assert.AreEqual("2", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
            tenantRequest = tenantRequest.ExecuteCommand(new ScheduleServiceWorkCommand(worker.PersonContactInfo.EmailAddress , DateTime.Today, 1));

            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand( new ReportServiceWorkCommand() { Notes = "Can't come" });
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            tenantRequest = tenantRequest.ExecuteCommand( new ScheduleServiceWorkCommand(worker1.PersonContactInfo.EmailAddress , DateTime.Today, 1));
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker1.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand(new ReportServiceWorkCommand() { Notes = "All done", Success =true});
            Assert.AreEqual(5, tenantRequest.RequestChanges.Count);
            tenantRequest.ExecuteCommand(new CloseTenantRequestCommand());
            Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
            tenantRequest = tenant.AddRequest(
                new RegisterTenantRequestCommand()
                {
                    Title = "Full renovation needed",
                    Description ="Needs painting everything"
                });
            Assert.AreEqual("3", tenantRequest.Code);
            tenantRequest = tenantRequest.ExecuteCommand(new RejectTenantRequestCommand() { Notes = "We can't do that" });

            tenantRequest.ExecuteCommand(new CloseTenantRequestCommand() );

        }

        [TestMethod]
        public void AllPathsTestWithInMemoryRepo()
        {
            var repo = new PropertyRepositoryInMemory();
            var emailService = new EmailServiceMock();
            var templateDataService = new TemplateDataService();
            var ntfService = new NotifyPartiesService(emailService, templateDataService,repo);

            TestEverythingWithRepo(repo, ntfService, emailService);
        }

        [TestMethod]
        public void AllPathsTestWithEntityFrameworkRepo()
        {

            const string connectionString = "Server=(localdb)\\mssqllocaldb; Database = PropertiesTest1; Trusted_Connection = True; MultipleActiveResultSets = true";

            var optionsBuilder = new DbContextOptionsBuilder<PropertiesContext>();
            optionsBuilder.UseSqlServer(connectionString);


            //PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options);

            // Or you can also instantiate inside using

            using (PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options))
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                //...do stuff
                var repo = new PropertyRepositoryEf(dbContext);
                var emailService = new EmailServiceMock();
                var templateDataService = new TemplateDataService();
                var ntfService = new NotifyPartiesService(emailService, templateDataService, repo);

                TestEverythingWithRepo(repo, ntfService, emailService);
            }


        }

        private static void TestEverythingWithRepo(IPropertyRepository repo, INotifyPartiesService ntfService,
            EmailServiceMock emailService)
        {
            
            var authService = new UserAuthorizationService(repo);
            var propService = new PropertyService(repo, ntfService, authService);
            const string superintendentLogin = "super@email.com";
            const string tenantLogin = "tenant9@email.com";
            const string workerLogin = "worker@email.com";
            const string worker1Login = "worker1@email.com";
           
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            propService.AddWorker(new PersonContactInfo()
            {
                LastName = "Douglas",
                FirstName = "Jordan",
                EmailAddress = workerLogin,
                MobilePhone = "647-222-2222"
            });
            propService.AddWorker(new PersonContactInfo()
            {
                LastName = "Lee",
                FirstName = "Pete",
                EmailAddress = worker1Login,
                MobilePhone = "647-222-2223"
            });
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);

            var prop = propService.AddProperty(new AddPropertyCommand("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" }, "noreply@moonlightapartments.com"));

            var workers = repo.GetAllWorkers().ToArray() ;
            Assert.AreEqual(2, workers.Count());

            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin);

            var tenant = propService.AddTenant(prop.Code,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com",
                    FirstName = "John",
                    LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );
            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin,prop.Code , tenant.UnitNumber  );
            var tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title =  "Power plug in kitchen",
                    Description = "The plug is broken"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("1", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);

            //----------
            var trId = tenantRequest.Id;
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );

            tenantRequest = propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));

            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(trId, tenantRequest.Id);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);

           //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success =true});
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin, tenant.PropertyCode, tenant.UnitNumber  );
            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title =  "Kitchen desk replace",
                    Description = "Kitchen desk is in awful condition"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("2", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "Can't come" });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[1].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[1].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success  = true});
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(5, tenantRequest.RequestChanges.Count);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest =
                propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );
            Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin, prop.Code, tenant.UnitNumber);
            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title = "Full renovation needed",
                    Description = "Needs painting everything"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("3", tenantRequest.Code);
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new RejectTenantRequestCommand() { Notes = "We can't do that" });
            AssertEmailPropsNotNull(emailService);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            propService.ExecuteTenantRequestCommand(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );

            propService.GetPropertyTenants("moonlight");


        }

        private static void AssertEmailPropsNotNull(EmailServiceMock emailService)
        {
            Assert.IsNotNull(emailService.LastSentEmail);
            Assert.IsNotNull(emailService.LastSentEmail.RecipientEmail);
            Assert.IsNotNull(emailService.LastSentEmail.SenderEmail);
            Assert.IsNotNull(emailService.LastSentEmail.Subject);
            Assert.IsNotNull(emailService.LastSentEmail.Body);
        }
    }
}
