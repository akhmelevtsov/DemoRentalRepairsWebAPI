using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Email;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Notifications.Interfaces;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Core.Tests.Integration
{

    [TestClass]
    public class TenantRequestTests
    {

        [TestMethod]
        public async Task ServiceHappyPathTestWithInMemoryRepoAndServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<ISecurityService, SecurityMockService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            //services.AddSingleton<IEmailService, MailSlurperEmailService  >();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();

            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>(); //new PropertyRepositoryInMemory();

            await SharedTests.TestHappyPath(repo, propertyService, authService);
            var emailService = (EmailServiceMock)serviceProvider.GetService<IEmailService>();
            Assert.IsNotNull(emailService.Emails);
            Assert.AreEqual(10, emailService.Emails.Count);

            foreach (var e in emailService.Emails)
            {
                Assert.AreEqual("demo-rental-repairs-no-reply@protonmail.com", e.SenderEmail);
                Assert.IsNotNull(e.RecipientEmail);
                Assert.IsNotNull(e.Body);
                Assert.IsNotNull(e.Subject);
            }




        }
        [TestMethod]
        public async Task ServiceHappyPathTestWithInMemoryRepoAndQueuedNotifications()
        {
            var services = new ServiceCollection();

            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<ISecurityService, SecurityMockService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddSingleton<IEmailService, EmailServiceMock>();
            services.AddSingleton<IQueueClient, QueueClientMock>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesQueueClient>();
            services.AddTransient< NotifyPartiesService> ();
            services.AddSingleton<IPropertyService, PropertyService>();

            var serviceProvider = services.BuildServiceProvider();

            var propertyService = serviceProvider.GetService<IPropertyService>();
            var authService = serviceProvider.GetService<IUserAuthorizationService>();

            var repo = serviceProvider.GetService<IPropertyRepository>(); //new PropertyRepositoryInMemory();

            await SharedTests.TestHappyPath(repo, propertyService, authService);

            var queueClient = serviceProvider.GetService<IQueueClient>();
            var messages = ((QueueClientMock) queueClient).Messages;

            Assert.AreEqual(13, messages.Count);

            var builder = serviceProvider.GetService<IEmailBuilderService >();

            var handler = new NotifyPartiesQueueHandler(builder );
            int count = 0;
            foreach (var m in messages)
            {

                var email = await handler.Handle(m);
                if (email != null)
                {
                    count++;
                    Assert.IsNotNull(email.Body);
                    Assert.IsNotNull(email.RecipientEmail);
                    Assert.IsNotNull(email.SenderEmail);
                    Assert.IsNotNull(email.Subject);
                }
         
            }
           

           Assert.AreEqual(10,count);
        }
  

      
        [TestMethod]
        public async Task ServiceHappyPathTestWithEfRepo()
        {

            const string connectionString = "Server=(localdb)\\mssqllocaldb; Database = PropertiesTest1; Trusted_Connection = True; MultipleActiveResultSets = true";

           
            var services = new ServiceCollection();
            services.AddDbContext<PropertiesContext>(options =>
                options.UseSqlServer(connectionString) );

            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<ISecurityService, SecurityMockService>();
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

            using (var context = serviceProvider.GetService<PropertiesContext>())
            {
                  context.Database.EnsureDeleted();
                  context.Database.EnsureCreated();
                 await SharedTests.TestHappyPath(repo, propertyService, authService);
            }

        }

        [TestMethod]
        public async Task ServiceHappyPathTestWithMongoDbRepo()
        {


            var services = new ServiceCollection();

            services.AddSingleton<IMongoDbSettings>(new RentalRepairsMongoDbSettings() { DatabaseName = "RentalRepairs", ConnectionString = "mongodb://localhost:27017"});
            services.AddSingleton<IMongoDbContext, RentalRepairsMongoDbContext>();


            services.AddTransient<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<ISecurityService, SecurityMockService>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();

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

            var dbContext = serviceProvider.GetService<IMongoDbContext> ();
            dbContext.DropDb();

            await SharedTests.TestHappyPath(repo, propertyService, authService);
           


        }
  
        [TestMethod]
        public async Task ServiceHappyPathTestWithEfRepoAndAspNetIdentity()
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
