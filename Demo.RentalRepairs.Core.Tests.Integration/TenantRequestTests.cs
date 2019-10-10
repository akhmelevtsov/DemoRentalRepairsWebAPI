using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories;
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

            var propService = new PropertyDomainService();

            var prop = propService.CreateProperty("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                    {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M95 4T1"},
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com", FirstName = "John", LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() {"11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34"});

            var tenant = propService.AddTenant(prop,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com", FirstName = "John", LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );


            var tenantRequest = propService.RegisterTenantRequest(tenant,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Power plug in kitchen", "Water leak in main bathroom"},

                });
            tenantRequest = tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Douglas", FirstName = "Jordan", EmailAddress = "worker@Workers.com",
                        MobilePhone = "647-222-2222"
                    },
                    ServiceDate = DateTime.Today, WorkOrderNo = 1, WorkerId = Guid.NewGuid()
                });
            tenantRequest =
                tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkCompleted,
                    new ServiceWorkReport() {Notes = "All done"});

            tenantRequest.ChangeStatus(TenantRequestStatusEnum.Closed, null);

            tenantRequest = propService.RegisterTenantRequest(tenant,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Kitchen desk replace"},

                });
            tenantRequest = tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Douglas", FirstName = "Jordan", EmailAddress = "worker@Workers.com",
                        MobilePhone = "647-222-2222"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            tenantRequest =
                tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkIncomplete,
                    new ServiceWorkReport() {Notes = "Can't come"});

            tenantRequest = tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Lee", FirstName = "Pete", EmailAddress = "worker2@Workers.com",
                        MobilePhone = "647-222-2223"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            tenantRequest =
                tenantRequest.ChangeStatus(TenantRequestStatusEnum.WorkCompleted,
                    new ServiceWorkReport() {Notes = "All done"});

            tenantRequest.ChangeStatus(TenantRequestStatusEnum.Closed, null);

            tenantRequest = propService.RegisterTenantRequest(tenant,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Full renovation needed"},

                });

            tenantRequest = tenantRequest.ChangeStatus(TenantRequestStatusEnum.RequestRejected,
                new TenantRequestRejectNotes() {Notes = "We can't do that"});

            tenantRequest.ChangeStatus(TenantRequestStatusEnum.Closed, null);

        }

        [TestMethod]
        public void AllPathsTestWithInMemoryRepo()
        {
            var repo = new PropertyRepositoryInMemory();
            var ntfService = new NotifyPartiesServiceMock();

            var propService = new PropertyService(repo, ntfService);
            var prop = propService.AddProperty("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                    {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M95 4T1"},
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() {"11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34"});

            var tenant = propService.AddTenant(prop.Code,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com", FirstName = "John", LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );


            var tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Power plug in kitchen", "Water leak in main bathroom"},

                });
            var trId = tenantRequest.Id;
            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Douglas", FirstName = "Jordan", EmailAddress = "worker@Workers.com",
                        MobilePhone = "647-222-2222"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            Assert.AreEqual(trId, tenantRequest.Id);

            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkCompleted, new ServiceWorkReport() {Notes = "All done"});

            propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.Closed, null);

            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Kitchen desk replace"},

                });
            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Douglas", FirstName = "Jordan", EmailAddress = "worker@Workers.com",
                        MobilePhone = "647-222-2222"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkIncomplete, new ServiceWorkReport() {Notes = "Can't come"});

            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code, TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Lee", FirstName = "Pete", EmailAddress = "worker2@Workers.com",
                        MobilePhone = "647-222-2223"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkCompleted, new ServiceWorkReport() {Notes = "All done"});
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.Closed, null);

            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Full renovation needed"},

                });

            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.RequestRejected, new TenantRequestRejectNotes() {Notes = "We can't do that"});

            propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.Closed, null);

        }

        [TestMethod]
        public void TestDomainValidations()
        {
            var propService = new PropertyDomainService();

            var prop = propService.CreateProperty("", "moonlight",
                new PropertyAddress()
                    {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M95 4T1"},
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1111"
                }, new List<string>() {"11" , "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34"});
            var tenant = propService.AddTenant(prop,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com",
                    FirstName = "John",
                    LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "22"
            );

        }

        [TestMethod]
        public void TestDomainUnitValidation()
        {

        }
    }
}
