using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;
using Demo.RentalRepairs.Tests.Integration.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Azure.Tests
{
    [TestClass]
    public class AzureCosmosDbRepoTest
    {
        [TestMethod]
        public async Task ServiceHappyPathTestWithCosmosDbRepo()
        {


            var services = new ServiceCollection();

            services.AddSingleton<IMongoDbSettings>(new RentalRepairsMongoDbSettings()
            {
                DatabaseName = "RentalRepairs",
                ConnectionString = ""
            });
            services.AddSingleton<IMongoDbContext, RentalRepairsCosmosDbContext>();


            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationMockService>();

            services.AddSingleton<IPropertyRepository, RentalRepairsMongoDbRepository>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();

            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();

            var repo = serviceProvider.GetService<IPropertyRepository>();

            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();


            var dbContext = serviceProvider.GetService<IMongoDbContext>();
            dbContext.DropDb();

            await SharedTests.TestHappyPath(repo, propertyService, authService);



        }
    }
}
