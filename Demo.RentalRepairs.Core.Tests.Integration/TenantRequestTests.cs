﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public async Task AllPathsTestWithInMemoryRepo()
        {
            var services = new ServiceCollection();
           
            //services.AddScoped<ISecurityService, SecurityService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();

            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>(); //new PropertyRepositoryInMemory();
            var emailService = serviceProvider.GetService<IEmailService>(); // new EmailServiceMock();
            var ntfService = serviceProvider.GetService<INotifyPartiesService>(); // new NotifyPartiesService(emailService, templateDataService,repo);

            await TestEverythingWithRepo(repo, ntfService, (EmailServiceMock )emailService, propertyService, authService );
        }

        [TestMethod]
        public async Task AllPathsTestWithEntityFrameworkRepo()
        {

            const string connectionString = "Server=(localdb)\\mssqllocaldb; Database = PropertiesTest1; Trusted_Connection = True; MultipleActiveResultSets = true";

            //var optionsBuilder = new DbContextOptionsBuilder<PropertiesContext>();
            //optionsBuilder.UseSqlServer(connectionString);


            ////PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options);

            //// Or you can also instantiate inside using

            //using (PropertiesContext dbContext = new PropertiesContext(optionsBuilder.Options))
            //{
            //    dbContext.Database.EnsureDeleted();
            //    dbContext.Database.EnsureCreated();
            //    //...do stuff
            //    var repo = new PropertyRepositoryEf(dbContext);
            //    var emailService = new EmailServiceMock();
            //    var templateDataService = new TemplateDataService();
            //    var ntfService = new NotifyPartiesService(emailService, templateDataService, repo);

            //    TestEverythingWithRepo(repo, ntfService, emailService, null , null);
            //}

            var services = new ServiceCollection();
            services.AddDbContext<PropertiesContext>(options =>
                options.UseSqlServer(connectionString) );

            //services.AddScoped<ISecurityService, SecurityService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryEf>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();
            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>(); 
            var emailService = serviceProvider.GetService<IEmailService>(); 
            var ntfService = serviceProvider.GetService<INotifyPartiesService>(); 

            using (var context = serviceProvider.GetService<PropertiesContext>())
            {
                  context.Database.EnsureDeleted();
                  context.Database.EnsureCreated();
                 await TestEverythingWithRepo(repo, ntfService, (EmailServiceMock)emailService, propertyService, authService);
            }
           

        }
        [TestMethod]
        public async Task AllPathsTestWithEfRepoAndAspNetIdentity()
        {

            const string connectionString = "Server=(localdb)\\mssqllocaldb; Database = PropertiesTest1; Trusted_Connection = True; MultipleActiveResultSets = true";

            const string conString =
                "Server=(localdb)\\mssqllocaldb;Database=aspnet-WebAuthApp-61133D25-72F3-4C13-AC8F-791FA643AA77;Trusted_Connection=True;MultipleActiveResultSets=true";
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(conString ));


            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                //.AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();



            services.AddDbContext<PropertiesContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryEf>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();
            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>();
            var emailService = serviceProvider.GetService<IEmailService>();
            var ntfService = serviceProvider.GetService<INotifyPartiesService>();
            var securityService = serviceProvider.GetService<ISecurityService>();
            using (var context1 = serviceProvider.GetService<ApplicationDbContext>())
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                using (var context = serviceProvider.GetService<PropertiesContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    await TestEverythingWithAspNetIdentity(repo, ntfService, (EmailServiceMock) emailService, propertyService,
                        authService);
                }
            }


        }
        private static  async  Task TestEverythingWithAspNetIdentity(IPropertyRepository repo, INotifyPartiesService ntfService,
     EmailServiceMock emailService, IPropertyService propService, IUserAuthorizationService authService)
        {

            
            const string superintendentLogin = "super@email.com";
            const string tenantLogin = "tenant9@email.com";
            const string workerLogin = "worker@email.com";
            const string worker1Login = "worker1@email.com";

            await authService.RegisterUser(UserRolesEnum.Worker, workerLogin, "Pass@22");

            await authService.GetUserClaims(workerLogin);

            await propService.AddWorkerAsync(new PersonContactInfo()
            {
                LastName = "Douglas",
                FirstName = "Jordan",
                EmailAddress = workerLogin,
                MobilePhone = "647-222-2222"
            });

            await authService.RegisterUser(UserRolesEnum.Worker, worker1Login, "Pass@22");

            await propService.AddWorkerAsync(new PersonContactInfo()
            {
                LastName = "Lee",
                FirstName = "Pete",
                EmailAddress = worker1Login,
                MobilePhone = "647-222-2223"
            });
            //----------
            await authService.RegisterUser(UserRolesEnum.Superintendent , superintendentLogin , "Pass@22");

            await authService.GetUserClaims(superintendentLogin);

            var prop = await propService.AddPropertyAsync(new AddPropertyCommand("Moonlight Apartments", "moonlight",
                new PropertyAddress()
                    { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                "905-111-1111",
                new PersonContactInfo()
                {
                    EmailAddress = superintendentLogin,
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "905-111-1112"
                }, new List<string>() { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" }, "noreply@moonlightapartments.com"));


            var workers = repo.GetAllWorkers().ToArray();
            Assert.AreEqual(2, workers.Count());

            //----------
            await authService.RegisterUser(UserRolesEnum.Tenant , tenantLogin , "Pass@22");

            await authService.GetUserClaims(tenantLogin);

            var tenant = await propService.AddTenantAsync(prop.Code,
                new PersonContactInfo()
                {
                    EmailAddress = tenantLogin ,
                    FirstName = "John",
                    LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                },
                "21"
            );
            //await securityService.SetLoggedUserClaims(tenantLogin, UserRolesEnum.Tenant , "moonlight", "21");

            //----------
            //authService.SetUser(UserRolesEnum.Tenant, tenantLogin, prop.Code, tenant.UnitNumber);
            await authService.GetUserClaims(tenantLogin);

            var tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title = "Power plug in kitchen",
                    Description = "The plug is broken"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("1", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);

            //----------
            var trId = tenantRequest.Id;
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);

            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));

            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(trId, tenantRequest.Id);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);

            //----------
            //authService.SetUser(UserRolesEnum.Worker, workerLogin);
            await authService.GetUserClaims(workerLogin);
            tenantRequest =
               await   propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success = true });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            //----------
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);
            tenantRequest = await  propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            //authService.SetUser(UserRolesEnum.Tenant, tenantLogin, tenant.PropertyCode, tenant.UnitNumber);
            await authService.GetUserClaims(tenantLogin);
            tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title = "Kitchen desk replace",
                    Description = "Kitchen desk is in awful condition"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("2", tenantRequest.Code);
            Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
            //----------
            // authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);

            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            //----------
            //authService.SetUser(UserRolesEnum.Worker, workerLogin);
            await authService.GetUserClaims(workerLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "Can't come" });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);

            //----------
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[1].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[1].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            //authService.SetUser(UserRolesEnum.Worker, workerLogin);
            await authService.GetUserClaims(workerLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success = true });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(5, tenantRequest.RequestChanges.Count);

            //----------
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());
            Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
            //----------
            //authService.SetUser(UserRolesEnum.Tenant, tenantLogin, prop.Code, tenant.UnitNumber);
            await authService.GetUserClaims(tenantLogin);
            tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title = "Full renovation needed",
                    Description = "Needs painting everything"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("3", tenantRequest.Code);
            //----------
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new RejectTenantRequestCommand() { Notes = "We can't do that" });
            AssertEmailPropsNotNull(emailService);

            //----------
            //authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code);
            await authService.GetUserClaims(superintendentLogin);
            await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());

            //propService.GetPropertyTenants("moonlight");


        }

        private static async  Task TestEverythingWithRepo(IPropertyRepository repo, INotifyPartiesService ntfService,
            EmailServiceMock  emailService, IPropertyService propService, IUserAuthorizationService authService)
        {

            //var services = new ServiceCollection();
            //services.AddTransient<IMatchRepository, MatchRepositoryStub>();

            //var serviceProvider = services.BuildServiceProvider();

            //matchRepository = serviceProvider.GetService<IMatchRepository>();


            //var authService = new UserAuthorizationService(repo);

            //ISecurityService securityService;


            //var securityService = new SecurityService(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext)), 
            //                                             authService, new RoleManager<IdentityRole>(), new SignInManager<ApplicationUser>(), new Logger<SecurityService>( )  )
            //var propService = new PropertyService(repo, ntfService, authService);
            const string superintendentLogin = "super@email.com";
            const string tenantLogin = "tenant9@email.com";
            const string workerLogin = "worker@email.com";
            const string worker1Login = "worker1@email.com";
           
            authService.SetUser(UserRolesEnum.Worker, workerLogin);

            await propService.AddWorkerAsync(new PersonContactInfo()
            {
                LastName = "Douglas",
                FirstName = "Jordan",
                EmailAddress = workerLogin,
                MobilePhone = "647-222-2222"
            });
            await propService.AddWorkerAsync(new PersonContactInfo()
            {
                LastName = "Lee",
                FirstName = "Pete",
                EmailAddress = worker1Login,
                MobilePhone = "647-222-2223"
            });
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin);

            var prop = await propService.AddPropertyAsync(new AddPropertyCommand("Moonlight Apartments", "moonlight",
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

            var tenant = await propService.AddTenantAsync(prop.Code,
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
            var tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
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

            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));

            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(trId, tenantRequest.Id);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);

           //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success =true});
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin, tenant.PropertyCode, tenant.UnitNumber  );
            tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
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
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "Can't come" });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(3, tenantRequest.RequestChanges.Count);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
                new ScheduleServiceWorkCommand(workers[1].PersonContactInfo.EmailAddress , DateTime.Today.AddDays(1), 1));
            Assert.AreEqual(workers[1].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
            Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Worker, workerLogin);
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success  = true});
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual(5, tenantRequest.RequestChanges.Count);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest =
                await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );
            Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
            //----------
            authService.SetUser(UserRolesEnum.Tenant, tenantLogin, prop.Code, tenant.UnitNumber);
            tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
                new RegisterTenantRequestCommand()
                {
                    Title = "Full renovation needed",
                    Description = "Needs painting everything"
                });
            AssertEmailPropsNotNull(emailService);
            Assert.AreEqual("3", tenantRequest.Code);
            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new RejectTenantRequestCommand() { Notes = "We can't do that" });
            AssertEmailPropsNotNull(emailService);

            //----------
            authService.SetUser(UserRolesEnum.Superintendent, superintendentLogin, prop.Code );
            await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand() );

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
