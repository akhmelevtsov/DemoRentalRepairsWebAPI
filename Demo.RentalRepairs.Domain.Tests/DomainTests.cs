using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.Tests.Mocks;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Domain.Tests
{
    [TestClass]
    public class DomainTests
    {
        [TestMethod]
        public void DomainHappyPathTest()
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

            var prop = new Property(new RegisterPropertyCommand("Moonlight Apartments", "moonlight",
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
                    Title = "Power plug in kitchen",
                    Description = "The plug is broken"

                });
            Assert.AreEqual(DateTimeProviderMock.CurrentDate, tenantRequest.DateCreated);
            Assert.IsNotNull(tenantRequest.Id);
            Assert.AreEqual("1", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);

            tenantRequest = tenantRequest.ExecuteCommand(new ScheduleServiceWorkCommand(worker.PersonContactInfo.EmailAddress, DateTime.Today, 1));
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand(new ReportServiceWorkCommand() { Notes = "All done", Success = true });
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            tenantRequest.ExecuteCommand(new CloseTenantRequestCommand());
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            tenantRequest = tenant.AddRequest(new RegisterTenantRequestCommand()
            {
                Title = "Kitchen desk replace",
                Description = "Kitchen desk is in awful condition"

            });
            Assert.AreEqual("2", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
            tenantRequest = tenantRequest.ExecuteCommand(new ScheduleServiceWorkCommand(worker.PersonContactInfo.EmailAddress, DateTime.Today, 1));

            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand(new ReportServiceWorkCommand() { Notes = "Can't come" });
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            tenantRequest = tenantRequest.ExecuteCommand(new ScheduleServiceWorkCommand(worker1.PersonContactInfo.EmailAddress, DateTime.Today, 1));
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            Assert.AreEqual(worker1.PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            tenantRequest =
                tenantRequest.ExecuteCommand(new ReportServiceWorkCommand() { Notes = "All done", Success = true });
            Assert.AreEqual(5, tenantRequest.RequestChanges.Count);
            tenantRequest.ExecuteCommand(new CloseTenantRequestCommand());
            Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
            tenantRequest = tenant.AddRequest(
                new RegisterTenantRequestCommand()
                {
                    Title = "Full renovation needed",
                    Description = "Needs painting everything"
                });
            Assert.AreEqual("3", tenantRequest.Code);
            tenantRequest = tenantRequest.ExecuteCommand(new RejectTenantRequestCommand() { Notes = "We can't do that" });

            tenantRequest.ExecuteCommand(new CloseTenantRequestCommand());

        }
    }
}
