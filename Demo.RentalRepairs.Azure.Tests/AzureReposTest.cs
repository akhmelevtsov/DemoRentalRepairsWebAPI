using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Core.Tests.Integration;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Azure.Tests
{
    [TestClass]
    public class AzureSqlReposTest
    {
        [TestMethod]
        public async Task ServiceHappyPathTestWithAzureSqlRepos()
        {

            const string connectionString = "Server=tcp:demorr-db-srv.database.windows.net,1433;Initial Catalog=demorr-db;Persist Security Info=False;User ID=alexadmin;Password=Pass@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            const string conString =
                "Server=tcp:demorr-db-srv.database.windows.net,1433;Initial Catalog=demorr-identity-db;Persist Security Info=False;User ID=alexadmin;Password=Pass@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(conString));


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
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();
            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>();

            using (var context1 = serviceProvider.GetService<ApplicationDbContext>())
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                using (var context = serviceProvider.GetService<PropertiesContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    await SharedTests.TestHappyPath(repo, propertyService,
                        authService);
                }
            }


        }
    }
}
