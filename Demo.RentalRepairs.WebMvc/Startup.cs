using System;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Email;
using Demo.RentalRepairs.Infrastructure.Identity;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi;
using Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;

using Demo.RentalRepairs.WebMvc.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Add this for IsDevelopment extension
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.RentalRepairs.WebMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
       
        ComponentsSettings _componentsSettings;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ComponentsSettings>(Configuration.GetSection("Components"));

            _componentsSettings = services.BuildServiceProvider().GetService<IOptions<ComponentsSettings>>().Value;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                // Add security for cookies
                options.Secure = CookieSecurePolicy.Always;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            });
  
            switch (_componentsSettings.RepositoryType )
            {
                case RepositoryTypeEnum.InMemory:
                    services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
                    break;
                case RepositoryTypeEnum.SqlServer:
                    services.AddDbContext<PropertiesContext>(options =>
                        options.UseSqlServer(
                            Configuration.GetConnectionString("DemoRentalRepairsWebMvcContext"),
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly("Demo.RentalRepairs.WebMvc");
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 5,
                                    maxRetryDelay: TimeSpan.FromSeconds(30),
                                    errorNumbersToAdd: null);
                            }));
                    services.AddTransient<IPropertyRepository, PropertyRepositoryEf>();
                    break;
                case RepositoryTypeEnum.MongoDb:
                    services.Configure<RentalRepairsMongoDbSettings>(
                        Configuration.GetSection(nameof(RentalRepairsMongoDbSettings)));

                    services.AddSingleton<IMongoDbSettings>(sp =>
                        sp.GetRequiredService<IOptions<RentalRepairsMongoDbSettings>>().Value);

                    services.AddSingleton<IMongoDbContext, RentalRepairsMongoDbContext>();
                    services.AddSingleton<IPropertyRepository, RentalRepairsMongoDbRepository>();


                    break;
                case RepositoryTypeEnum.CosmosDb:
                    services.Configure<RentalRepairsCosmosDbSettings>(
                        Configuration.GetSection(nameof(RentalRepairsCosmosDbSettings)));

                    services.AddSingleton<IMongoDbSettings>(sp =>
                        sp.GetRequiredService<IOptions<RentalRepairsCosmosDbSettings>>().Value);

                    services.AddSingleton<IMongoDbContext, RentalRepairsCosmosDbContext>();
                    services.AddSingleton<IPropertyRepository, RentalRepairsMongoDbRepository>();


                    break;
                case RepositoryTypeEnum.AzureTable:
                    services.Configure<RentalRepairsAzureTableApiDbSettings>(
                        Configuration.GetSection(nameof(RentalRepairsAzureTableApiDbSettings)));
                    services.AddSingleton(sp =>
                        sp.GetRequiredService<IOptions<RentalRepairsAzureTableApiDbSettings>>().Value);
                    services.AddSingleton<AzureTableApiDbContext>();
                    services.AddSingleton<IPropertyRepository, AzureTableApiRepository>();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Updated for .NET 8 - remove SetCompatibilityVersion and configure MVC properly
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                
                // Add anti-forgery token to all POST requests
                config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
           
            //services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
           
            switch (_componentsSettings.IdentityType )
            {
                case IdentityTypeEnum.AspNetIdentity:
                    services.AddAspNetIdentityServices(Configuration);
                    break;
                case IdentityTypeEnum.AzureAdB2C:
                    services.AddAdB2CServices(Configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            services.AddTransient<ITemplateDataService, TemplateDataService>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();

            switch (_componentsSettings.EmailServiceType )
            {
                case EmailServiceTypeEnum.NoEmail:
                    services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
                    services.AddTransient<IEmailService, EmailServiceMock>();
                    break;
                case EmailServiceTypeEnum.DebugEmailSlurper:
                    services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
                    services.AddTransient<IEmailService, MailSlurperEmailService>();
                    break;
                case EmailServiceTypeEnum.AzureSendGrid:
                    services.AddTransient<INotifyPartiesService, NotifyPartiesQueueClient>();
                    services.AddTransient<IEmailService, EmailServiceMock>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            services.AddScoped<IPropertyService, PropertyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            // Run automatic migrations
            RunMigrations(serviceProvider, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Add security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Add("Content-Security-Policy", 
                    "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            
            app.UseRouting(); // Add this for .NET 8
            
            app.UseAuthentication();
            app.UseAuthorization(); // Add this for .NET 8
            app.UseSession();
            
            if (_componentsSettings.IdentityType == IdentityTypeEnum.AzureAdB2C)
                app.UseRewriter(
                new RewriteOptions().Add(
                    context =>
                    {
                        if (context.HttpContext.Request.Path == "/AzureADB2C/Account/SignedOut")
                        { context.HttpContext.Response.Redirect("/Home/AfterSignOut"); }
                    })
            );
            
            // Updated for .NET 8 - use MapControllerRoute instead of UseMvc
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area}/{controller}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id:alpha?}");
            });
        }

        private void RunMigrations(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
            
            try
            {
                // Run migrations for ASP.NET Identity if using AspNetIdentity
                if (_componentsSettings.IdentityType == IdentityTypeEnum.AspNetIdentity)
                {
                    logger.LogInformation("Running migrations for ApplicationDbContext (ASP.NET Identity)...");
                    var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    try
                    {
                        // First, ensure the database is created
                        logger.LogInformation("Ensuring Identity database exists...");
                        identityContext.Database.EnsureCreated();
                        
                        // Then run migrations
                        logger.LogInformation("Running Identity migrations...");
                        identityContext.Database.Migrate();
                        
                        logger.LogInformation("ApplicationDbContext migrations completed successfully.");
                    }
                    catch (Exception identityEx)
                    {
                        logger.LogError(identityEx, "Failed to migrate Identity database. Attempting to create database manually...");
                        
                        // Try to create database without migrations first
                        try
                        {
                            identityContext.Database.EnsureCreated();
                            logger.LogInformation("Identity database created successfully without migrations.");
                        }
                        catch (Exception createEx)
                        {
                            logger.LogError(createEx, "Failed to create Identity database.");
                            if (!env.IsDevelopment()) throw;
                        }
                    }
                }

                // Run migrations for Properties context if using SqlServer repository
                if (_componentsSettings.RepositoryType == RepositoryTypeEnum.SqlServer)
                {
                    logger.LogInformation("Running migrations for PropertiesContext...");
                    var propertiesContext = scope.ServiceProvider.GetRequiredService<PropertiesContext>();
                    
                    try
                    {
                        // First, ensure the database is created
                        logger.LogInformation("Ensuring Properties database exists...");
                        propertiesContext.Database.EnsureCreated();
                        
                        // Then run migrations
                        logger.LogInformation("Running Properties migrations...");
                        propertiesContext.Database.Migrate();
                        
                        logger.LogInformation("PropertiesContext migrations completed successfully.");
                    }
                    catch (Exception propertiesEx)
                    {
                        logger.LogError(propertiesEx, "Failed to migrate Properties database. Attempting to create database manually...");
                        
                        // Try to create database without migrations first
                        try
                        {
                            propertiesContext.Database.EnsureCreated();
                            logger.LogInformation("Properties database created successfully without migrations.");
                        }
                        catch (Exception createEx)
                        {
                            logger.LogError(createEx, "Failed to create Properties database.");
                            if (!env.IsDevelopment()) throw;
                        }
                    }
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                logger.LogError(sqlEx, "SQL Server error occurred while running migrations. Error Number: {ErrorNumber}, State: {State}, Class: {Class}", 
                    sqlEx.Number, sqlEx.State, sqlEx.Class);
                
                // Specific handling for authentication issues
                if (sqlEx.Number == 17892 || sqlEx.Number == 18456)
                {
                    logger.LogError("Authentication failed. This might be due to:");
                    logger.LogError("1. SQL Server LocalDB not running - Try: sqllocaldb start MSSQLLocalDB");
                    logger.LogError("2. Windows authentication issues");
                    logger.LogError("3. Database access restrictions");
                    logger.LogError("4. LocalDB instance needs to be created - Try: sqllocaldb create MSSQLLocalDB");
                }
                
                // In development, continue running but log the error
                if (env.IsDevelopment())
                {
                    logger.LogWarning("Continuing application startup despite migration failure in development mode.");
                    logger.LogWarning("You may need to run migrations manually: dotnet ef database update");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while running migrations.");
                
                // In development, you might want the application to continue running even if migrations fail
                // In production, you might want to throw the exception to prevent startup
                if (!env.IsDevelopment())
                {
                    throw;
                }
                else
                {
                    logger.LogWarning("Continuing application startup despite migration failure in development mode.");
                    logger.LogWarning("You may need to run migrations manually: dotnet ef database update");
                }
            }
        }
    }
}
