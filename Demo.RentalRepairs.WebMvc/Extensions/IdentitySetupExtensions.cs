using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.WebMvc.Interfaces;
using Demo.RentalRepairs.WebMvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Demo.RentalRepairs.WebMvc.Extensions
{
    public static class IdentitySetupExtensions
    {
 
        public static IServiceCollection AddAspNetIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));


            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTenantRole",
                    policy => policy.RequireRole("Tenant"));
                options.AddPolicy("RequireSuperintendentRole",
                    policy => policy.RequireRole("Superintendent"));
                options.AddPolicy("RequireWorkerRole",
                    policy => policy.RequireRole("Worker"));
                options.AddPolicy("RequireAnonymousRole",
                    policy => policy.RequireRole("Anonymous"));
            });
            services.AddScoped<ISecuritySignInService, AspNetIdentitySignInService>();
            services.AddScoped<IUserAuthorizationService, AspNetIdentityService>();
            services.AddScoped<IIdentityRedirectionService, AspIdentityRedirectionService>();
            return services;

        }
        public static IServiceCollection AddAdB2CServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
                .AddAzureADB2C(options => configuration.Bind("AzureAdB2C", options));

            services.Configure<AzureAdB2CSettings>(configuration.GetSection(nameof(AzureAdB2CSettings)));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTenantRole",
                    policy => policy.RequireClaim("extension_userrole", "Tenant"));
                options.AddPolicy("RequireSuperintendentRole",
                    policy => policy.RequireClaim("extension_userrole", "Superintendent"));
                options.AddPolicy("RequireWorkerRole",
                    policy => policy.RequireClaim("extension_userrole", "Worker"));
                options.AddPolicy("RequireAnonymousRole",
                    policy => policy.RequireAssertion(ctx => !ctx.User.HasClaim(x => x.Type == "extension_userrole")));
            });
            services.AddScoped<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<IUserAuthorizationService, AzureAdB2CIdentityService>();
            services.AddScoped<IIdentityRedirectionService, AzureAdB2CRedirectionService>();
            return services;
        }
    }
}
