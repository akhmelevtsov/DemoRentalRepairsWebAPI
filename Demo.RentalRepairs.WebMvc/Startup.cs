using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Email;
using Demo.RentalRepairs.Infrastructure.Identity;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Repositories;
using Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi;
using Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;

using Demo.RentalRepairs.WebMvc.Extensions;
using Demo.RentalRepairs.WebMvc.Interfaces;
using Demo.RentalRepairs.WebMvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            });
  
            switch (_componentsSettings.RepositoryType )
            {
                case RepositoryTypeEnum.InMemory:
                    services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
                    break;
                case RepositoryTypeEnum.SqlServer:
                    services.AddDbContext<PropertiesContext>(options =>
                        options.UseSqlServer(
                            Configuration.GetConnectionString("DemoRentalRepairsWebMvcContext")));
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


            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
           
            //services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                //options.Cookie.HttpOnly = true;
                //// Make the session cookie essential
                //options.Cookie.IsEssential = true;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseAuthentication();
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
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id:alpha?}");

            });
           
        }
       
    }
}
