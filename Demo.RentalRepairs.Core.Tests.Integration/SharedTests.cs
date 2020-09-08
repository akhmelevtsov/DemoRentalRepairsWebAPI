using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Core.Tests.Integration
{
    //public class SharedTests
    //{
    //    public static async Task TestHappyPath(IPropertyRepository repo, IPropertyService propService, IUserAuthorizationService authService,
    //         string superintendentLogin = "superintendent1234@malinator.com",
    //         string tenantLogin = "tenant1234@malinator.com",
    //         string workerLogin = "worker1234@malinator.com",
    //         string worker1Login = "worker1235@malinator.com",
    //         string noReplyEmailAddress = "demo-rental-repairs-no-reply@protonmail.com")
    //    { 

    //        await authService.RegisterUser(UserRolesEnum.Worker, workerLogin, "Pass@22");

    //        await authService.GetUserClaims(workerLogin);

    //        await propService.RegisterWorkerAsync(new PersonContactInfo()
    //        {
    //            LastName = "Douglas",
    //            FirstName = "Jordan",
    //            EmailAddress = workerLogin,
    //            MobilePhone = "647-222-2222"
    //        });

    //        await authService.RegisterUser(UserRolesEnum.Worker, worker1Login, "Pass@22");

    //        await propService.RegisterWorkerAsync(new PersonContactInfo()
    //        {
    //            LastName = "Lee",
    //            FirstName = "Pete",
    //            EmailAddress = worker1Login,
    //            MobilePhone = "647-222-2223"
    //        });
    //        //----------
    //        await authService.RegisterUser(UserRolesEnum.Superintendent, superintendentLogin, "Pass@22");

    //        await authService.GetUserClaims(superintendentLogin);


    //        var prop = await propService.RegisterPropertyAsync(new RegisterPropertyCommand("Moonlight Apartments", "moonlight",
    //            new PropertyAddress()
    //            { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
    //            "905-111-1111",
    //            new PersonContactInfo()
    //            {
    //                EmailAddress = superintendentLogin,
    //                FirstName = "John",
    //                LastName = "Smith",
    //                MobilePhone = "905-111-1112"
    //            }, new List<string>() { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" }, noReplyEmailAddress));


    //        var workers = repo.GetAllWorkers().ToArray();
    //        Assert.AreEqual(2, workers.Count());

    //        //----------
    //        await authService.RegisterUser(UserRolesEnum.Tenant, tenantLogin, "Pass@22");

    //        await authService.GetUserClaims(tenantLogin);

    //        var tenant = await propService.RegisterTenantAsync(prop.Code,
    //            new PersonContactInfo()
    //            {
    //                EmailAddress = tenantLogin,
    //                FirstName = "John",
    //                LastName = "Tenant",
    //                MobilePhone = "222-222-2222"
    //            },
    //            "21"
    //        );
    //        //----------

    //        await authService.GetUserClaims(tenantLogin);

    //        var tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
    //            new RegisterTenantRequestCommand()
    //            {
    //                Title = "Power plug in kitchen",
    //                Description = "The plug is broken"
    //            });
            
    //        Assert.AreEqual("1", tenantRequest.Code);
    //        Assert.AreEqual(1, tenantRequest.RequestChanges.Count);

    //        //----------
    //        var trId = tenantRequest.Id;

    //        await authService.GetUserClaims(superintendentLogin);

    //        tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
    //            new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));

    //        Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
    //        Assert.AreEqual(trId, tenantRequest.Id);
          
    //        Assert.AreEqual(2, tenantRequest.RequestChanges.Count);

    //        //----------

    //        await authService.GetUserClaims(workerLogin);
    //        tenantRequest =
    //           await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success = true });
           
    //        Assert.AreEqual(3, tenantRequest.RequestChanges.Count);
    //        //----------
           
    //        await authService.GetUserClaims(superintendentLogin);
    //        tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());
    //        Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
    //        //----------
       
    //        await authService.GetUserClaims(tenantLogin);
    //        tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
    //            new RegisterTenantRequestCommand()
    //            {
    //                Title = "Kitchen desk replace",
    //                Description = "Kitchen desk is in awful condition"
    //            });
          
    //        Assert.AreEqual("2", tenantRequest.Code);
    //        Assert.AreEqual(1, tenantRequest.RequestChanges.Count);
    //        //----------
         
    //        await authService.GetUserClaims(superintendentLogin);

    //        tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
    //            new ScheduleServiceWorkCommand(workers[0].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));
    //        Assert.AreEqual(workers[0].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
           
    //        Assert.AreEqual(2, tenantRequest.RequestChanges.Count);
    //        //----------
      
    //        await authService.GetUserClaims(workerLogin);
    //        tenantRequest =
    //            await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "Can't come" });
    //        Assert.AreEqual(3, tenantRequest.RequestChanges.Count);

    //        //----------
            
    //        await authService.GetUserClaims(superintendentLogin);
    //        tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code,
    //            new ScheduleServiceWorkCommand(workers[1].PersonContactInfo.EmailAddress, DateTime.Today.AddDays(1), 1));
    //        Assert.AreEqual(workers[1].PersonContactInfo.EmailAddress, tenantRequest.WorkerEmail);
    //        Assert.AreEqual(4, tenantRequest.RequestChanges.Count);
    //        //----------
           
    //        await authService.GetUserClaims(workerLogin);
    //        tenantRequest =
    //            await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new ReportServiceWorkCommand() { Notes = "All done", Success = true });
    //        Assert.AreEqual(5, tenantRequest.RequestChanges.Count);

    //        //----------
           
    //        await authService.GetUserClaims(superintendentLogin);
    //        tenantRequest =
    //            await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());
    //        Assert.AreEqual(6, tenantRequest.RequestChanges.Count);
    //        //----------
       
    //        await authService.GetUserClaims(tenantLogin);
    //        tenantRequest = await propService.RegisterTenantRequestAsync(prop.Code, tenant.UnitNumber,
    //            new RegisterTenantRequestCommand()
    //            {
    //                Title = "Full renovation needed",
    //                Description = "Needs painting everything"
    //            });
    //        Assert.AreEqual("3", tenantRequest.Code);
    //        //----------
          
    //        await authService.GetUserClaims(superintendentLogin);
    //        tenantRequest = await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new RejectTenantRequestCommand() { Notes = "We can't do that" });

    //        //----------
        
    //        await authService.GetUserClaims(superintendentLogin);
    //        await propService.ExecuteTenantRequestCommandAsync(prop.Code, tenant.UnitNumber, tenantRequest.Code, new CloseTenantRequestCommand());

    //        //propService.GetPropertyTenants("moonlight");


    //    }
    //}
}
