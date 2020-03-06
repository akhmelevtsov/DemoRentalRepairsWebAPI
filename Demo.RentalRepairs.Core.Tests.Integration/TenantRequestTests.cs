using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
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

            var propService = new PropertyDomainService(new DateTimeProviderMock());

            var prop = propService.CreateProperty("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                    {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5"},
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com", FirstName = "John", LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() {"11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34"});

            Assert.AreEqual(DateTimeProviderMock.CurrentDate , prop.DateCreated );

            var tenant = propService.AddTenant(prop,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com", FirstName = "John", LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );

            Assert.AreEqual(DateTimeProviderMock.CurrentDate, tenant.DateCreated);

            var tenantRequest = propService.RegisterTenantRequest(tenant,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Power plug in kitchen", "Water leak in main bathroom"},

                });
            Assert.AreEqual(DateTimeProviderMock.CurrentDate, tenantRequest.DateCreated);

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

            TestEverythingWithRepo(repo, ntfService);
        }

        [TestMethod]
        public void AllPathsTestWithEntityFrameworkRepo()
        {

            const string connectionString = "Server=(localdb)\\mssqllocaldb; Database = PropertiesTest; Trusted_Connection = True; MultipleActiveResultSets = true";

            var optionsBuilder = new DbContextOptionsBuilder<PropertiesContext>();
            optionsBuilder.UseSqlServer(connectionString);


            //PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options);

            // Or you can also instantiate inside using

            using (PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options))
            {
                dbContext.Database.EnsureDeleted() ;
                dbContext.Database.EnsureCreated();
                //...do stuff
                var repo = new PropertyRepositoryEntityFramework(dbContext);
                var ntfService = new NotifyPartiesServiceMock();

                TestEverythingWithRepo(repo, ntfService);
            }

            
        }

        private static void TestEverythingWithRepo(IPropertyRepository repo, INotifyPartiesService ntfService)
        {

            var authService = new UserAuthCoreService(repo );
            var propService = new PropertyService(repo, ntfService, authService);
            const string superintendentLogin = "super@email.com";
            const string tenantLogin = "tenant@email.com";
            const string workerLogin = "worker@email.com";

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
           
            var prop = propService.AddProperty("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                    {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5"},
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() {"11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34"});

            authService.SetUser(UserRolesEnum.Tenant, tenantLogin  );

            var tenant = propService.AddTenant(prop.Code,
                new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com", FirstName = "John", LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin);
            var tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Power plug in kitchen", "Water leak in main bathroom"},
                });

            var trId = tenantRequest.Id;
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.WorkScheduled,
                new ServiceWorkOrder()
                {
                    Person = new PersonContactInfo()
                    {
                        LastName = "Douglas", FirstName = "Jordan", EmailAddress = workerLogin,
                        MobilePhone = "647-222-2222"
                    },
                    ServiceDate = DateTime.Today,
                    WorkOrderNo = 1,
                    WorkerId = Guid.NewGuid()
                });
            Assert.AreEqual(trId, tenantRequest.Id);
            authService.SetUser(UserRolesEnum.Worker,workerLogin );
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkCompleted, new ServiceWorkReport() {Notes = "All done"});

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.Closed, null);

            authService.SetUser(UserRolesEnum.Tenant, tenantLogin);
            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Kitchen desk replace"},
                });

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
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

            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkIncomplete, new ServiceWorkReport() {Notes = "Can't come"});

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.WorkScheduled,
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

            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.WorkCompleted, new ServiceWorkReport() {Notes = "All done"});

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            tenantRequest =
                propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                    TenantRequestStatusEnum.Closed, null);

            authService.SetUser(UserRolesEnum.Tenant, tenantLogin);
            tenantRequest = propService.RegisterTenantRequest(prop.Code, tenant.UnitNumber,
                new TenantRequestDoc()
                {
                    RequestItems = new string[] {"Full renovation needed"},
                });

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            tenantRequest = propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.RequestRejected, new TenantRequestRejectNotes() {Notes = "We can't do that"});

            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);
            propService.ChangeRequestStatus(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                TenantRequestStatusEnum.Closed, null);

            propService.GetPropertyTenants("moonlight");
        }

 
    }
}
