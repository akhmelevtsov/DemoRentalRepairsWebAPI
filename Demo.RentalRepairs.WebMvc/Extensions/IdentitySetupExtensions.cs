using System;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C;
using Demo.RentalRepairs.Infrastructure.Mocks;
using Demo.RentalRepairs.WebMvc.Authorization;
using Demo.RentalRepairs.WebMvc.Interfaces;
using Demo.RentalRepairs.WebMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace Demo.RentalRepairs.WebMvc.Extensions
{
    public static class IdentitySetupExtensions
    {
 
        public static IServiceCollection AddAspNetIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("Demo.RentalRepairs.WebMvc");
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTenantRole",
                    policy => policy.RequireRole("Tenant"));
                options.AddPolicy("RequireSuperintendentRole",
                    policy => policy.RequireRole("Superintendent"));
                options.AddPolicy("RequireWorkerRole",
                    policy => 
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Worker");
                        policy.Requirements.Add(new WorkerExistsRequirement());
                    });
                // Remove RequireAnonymousRole - this is contradictory
                // Anonymous users should use [AllowAnonymous] instead
            });
            
            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler, WorkerExistsAuthorizationHandler>();
            
            // Register the mock email sender for demo purposes
            services.AddTransient<IEmailSender, MockEmailSender>();
            
            services.AddScoped<ISecuritySignInService, AspNetIdentitySignInService>();
            services.AddScoped<IUserAuthorizationService, AspNetIdentityService>();
            services.AddScoped<IIdentityRedirectionService, AspIdentityRedirectionService>();
            return services;

        }
        public static IServiceCollection AddAdB2CServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAdB2C");

            services.Configure<AzureAdB2CSettings>(configuration.GetSection(nameof(AzureAdB2CSettings)));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTenantRole",
                    policy => policy.RequireClaim("extension_userrole", "Tenant"));
                options.AddPolicy("RequireSuperintendentRole",
                    policy => policy.RequireClaim("extension_userrole", "Superintendent"));
                options.AddPolicy("RequireWorkerRole",
                    policy => 
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("extension_userrole", "Worker");
                        policy.Requirements.Add(new WorkerExistsRequirement());
                    });
                // Remove RequireAnonymousRole for B2C as well
            });
            
            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler, WorkerExistsAuthorizationHandler>();
            
            // Register the mock email sender for demo purposes
            services.AddTransient<IEmailSender, MockEmailSender>();
            
            services.AddScoped<ISecuritySignInService, SecuritySignInMockService>();
            services.AddScoped<IUserAuthorizationService, AzureAdB2CIdentityService>();
            services.AddScoped<IIdentityRedirectionService, AzureAdB2CRedirectionService>();
            return services;
        }
    }
}
