using System;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.Infrastructure.Repositories.EF;
using Demo.RentalRepairs.WebMvc.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<PropertiesContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DemoRentalRepairsWebMvcContext")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //    .AddFluentValidation(fv =>
            //    {
            //        fv.ConfigureClientsideValidation(enabled: false);
            //        fv.RegisterValidatorsFromAssemblyContaining<PropertyValidator>();
            //        fv.ImplicitlyValidateChildProperties = true;

            //    });
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            ////services.AddTransient<IValidator<PropertyModel>, PropertyValidator>();
            //services.AddTransient<IValidator<PropertyCodeValidator>>();
            services.AddTransient<IUserAuthDomainService, UserAuthDomainMockService>();
            //services.AddTransient<IUserAuthDomainService, UserAuthDomainService>();
            services.AddTransient<IUserAuthCoreService, UserAuthorizationMockService>();
            //services.AddTransient<IUserAuthCoreService, UserAuthCoreService>();
            //services.AddSingleton<IPropertyRepository, PropertyRepositoryInMemory>();
            services.AddTransient<IPropertyRepository, PropertyRepositoryEntityFramework>();
            services.AddTransient<INotifyPartiesService, NotifyPartiesServiceMock>();
            services.AddTransient<IPropertyService, PropertyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
        }
    }
}
