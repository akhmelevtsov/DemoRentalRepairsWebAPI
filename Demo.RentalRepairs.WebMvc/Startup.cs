using System;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Infrastructure;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<PropertiesContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DemoRentalRepairsWebMvcContext")));

            //services.Configure<RentalRepairsMongoDbSettings>(
            //    Configuration.GetSection(nameof(RentalRepairsMongoDbSettings)));

            //services.AddSingleton<IMongoDbSettings>(sp =>
            //    sp.GetRequiredService<IOptions<RentalRepairsMongoDbSettings>>().Value);
            //services.AddSingleton<IMongoDbContext, RentalRepairsMongoDbContext>();

            services.Configure<RentalRepairsCosmosDbSettings>(
                Configuration.GetSection(nameof(RentalRepairsCosmosDbSettings)));

            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<RentalRepairsCosmosDbSettings>>().Value);
            services.AddSingleton<IMongoDbContext, RentalRepairsCosmosDbContext>();

            services.AddMvc(config =>
            {
                // using Microsoft.AspNetCore.Mvc.Authorization;
                // using Microsoft.AspNetCore.Authorization;
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //    .AddFluentValidation(fv =>
            //    {
            //        fv.ConfigureClientsideValidation(enabled: false);
            //        fv.RegisterValidatorsFromAssemblyContaining<PropertyValidator>();
            //        fv.ImplicitlyValidateChildProperties = true;

            //    });
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTenantRole",
                    policy => policy.RequireRole("Tenant"));
                options.AddPolicy("RequireSuperintendentRole",
                    policy => policy.RequireRole("Superintendent"));
                options.AddPolicy("RequireWorkerRole",
                    policy => policy.RequireRole("Worker"));
            });


            ////services.AddTransient<IValidator<PropertyModel>, PropertyValidator>();
            //services.AddTransient<IValidator<PropertyCodeValidator>>();
            services.AddScoped<ISecuritySignInService , SecuritySignInService >();
            services.AddScoped<ISecurityService,SecurityService >();
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            //services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            //services.AddTransient<IPropertyRepository, PropertyRepositoryEf>();
            services.AddSingleton<IPropertyRepository, RentalRepairsMongoDbRepository>();
            services.AddTransient<ITemplateDataService , TemplateDataService >();
            services.AddTransient<IEmailService, EmailServiceMock>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesService>();
            services.AddScoped< IPropertyService, PropertyService>();
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
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id:alpha?}");
               
            });
            //CreateUserRoles(serviceProvider).Wait();
        }
        //private async Task CreateUserRoles(IServiceProvider serviceProvider)
        //{
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //    IdentityResult roleResult;
        //    //Adding Admin Role
        //    var roleCheck = await roleManager.RoleExistsAsync("Admin");
        //    if (!roleCheck)
        //    {
        //        //create the roles and seed them to the database
        //        roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
        //    }
        //    //Assign Admin role to the main User here we have given our newly registered 
        //    //login id for Admin management
        //    ApplicationUser user = await userManager.FindByEmailAsync("a@email.com");
        //    await userManager.AddToRoleAsync(user, "Admin");
        //}
    }
}
