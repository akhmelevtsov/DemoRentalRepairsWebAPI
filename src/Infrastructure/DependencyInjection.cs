using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Infrastructure.ApiIntegration;
using RentalRepairs.Infrastructure.Authentication;
using RentalRepairs.Infrastructure.Caching;
using RentalRepairs.Infrastructure.Configuration;
using RentalRepairs.Infrastructure.Monitoring;
using RentalRepairs.Infrastructure.Persistence;
using RentalRepairs.Infrastructure.Persistence.Repositories;
using RentalRepairs.Infrastructure.Services;
using RentalRepairs.Infrastructure.Services.Email;
using RentalRepairs.Infrastructure.Services.Notifications;
using SendGrid.Extensions.DependencyInjection;

namespace RentalRepairs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            }
            else
            {
                // Fallback to in-memory database for development/testing
                options.UseInMemoryDatabase("RentalRepairsDb");
            }
        });

        // Register DbContext interface
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Repository Registration
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantRequestRepository, TenantRequestRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();

        // Infrastructure Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();

        // External Services Configuration
        services.AddExternalServices(configuration);

        // Step 13: Infrastructure-Specific Concerns
        services.AddAuthenticationServices(configuration);
        services.AddCachingServices(configuration);
        services.AddMonitoringServices(configuration);
        services.AddApiIntegrationServices(configuration);

        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration sections
        var notificationSettings = new NotificationSettings();
        configuration.GetSection(NotificationSettings.SectionName).Bind(notificationSettings);
        services.AddSingleton(notificationSettings);

        var externalServicesSettings = new ExternalServicesSettings();
        configuration.GetSection(ExternalServicesSettings.SectionName).Bind(externalServicesSettings);
        services.AddSingleton(externalServicesSettings);

        // Register notification settings adapter
        services.AddSingleton<INotificationSettings>(provider => 
            new NotificationSettingsAdapter(provider.GetRequiredService<NotificationSettings>()));

        // Register email services based on configuration
        services.AddEmailServices(notificationSettings, externalServicesSettings);

        // Register notification services
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services, 
        NotificationSettings notificationSettings,
        ExternalServicesSettings externalServicesSettings)
    {
        switch (notificationSettings.EmailProvider)
        {
            case EmailProvider.Mock:
                services.AddSingleton<IEmailService, MockEmailService>();
                break;

            case EmailProvider.Smtp:
                var smtpOptions = new SmtpEmailOptions
                {
                    Host = externalServicesSettings.Smtp.Host,
                    Port = externalServicesSettings.Smtp.Port,
                    EnableSsl = externalServicesSettings.Smtp.EnableSsl,
                    EnableAuthentication = externalServicesSettings.Smtp.EnableAuthentication,
                    Username = externalServicesSettings.Smtp.Username,
                    Password = externalServicesSettings.Smtp.Password,
                    DefaultSenderEmail = notificationSettings.DefaultSenderEmail,
                    DefaultSenderName = notificationSettings.DefaultSenderName
                };
                services.AddSingleton(smtpOptions);
                services.AddScoped<IEmailService, SmtpEmailService>();
                break;

            case EmailProvider.SendGrid:
                // Register SendGrid client
                services.AddSendGrid(options =>
                {
                    options.ApiKey = externalServicesSettings.SendGrid.ApiKey;
                });

                var sendGridOptions = new SendGridEmailOptions
                {
                    ApiKey = externalServicesSettings.SendGrid.ApiKey,
                    DefaultSenderEmail = notificationSettings.DefaultSenderEmail,
                    DefaultSenderName = notificationSettings.DefaultSenderName
                };
                services.AddSingleton(sendGridOptions);
                services.AddScoped<IEmailService, SendGridEmailService>();
                break;

            default:
                // Default to mock service
                services.AddSingleton<IEmailService, MockEmailService>();
                break;
        }

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add HTTP context accessor for authentication - simplified implementation for testing
        services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, SimpleHttpContextAccessor>();

        // Register authentication services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }

    private static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind cache settings
        var cacheSettings = new CacheSettings();
        configuration.GetSection(CacheSettings.SectionName).Bind(cacheSettings);
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));

        // Register caching services based on configuration
        if (!cacheSettings.EnableCaching)
        {
            services.AddSingleton<ICacheService, NullCacheService>();
        }
        else
        {
            switch (cacheSettings.Provider.ToLowerInvariant())
            {
                case "memory":
                    services.AddMemoryCache();
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
                case "redis":
                    // TODO: Add Redis caching support in future iterations
                    services.AddMemoryCache();
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
                default:
                    services.AddMemoryCache();
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
            }
        }

        // Register cache decorators
        services.AddScoped<CachedPropertyService>();

        return services;
    }

    private static IServiceCollection AddMonitoringServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register performance monitoring
        services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();

        return services;
    }

    private static IServiceCollection AddApiIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HTTP client for external API calls
        services.AddHttpClient<IExternalApiClient, HttpExternalApiClient>(client =>
        {
            var externalServicesSettings = new ExternalServicesSettings();
            configuration.GetSection(ExternalServicesSettings.SectionName).Bind(externalServicesSettings);

            if (!string.IsNullOrEmpty(externalServicesSettings.ApiIntegrations.WorkerSchedulingApiUrl))
            {
                client.BaseAddress = new Uri(externalServicesSettings.ApiIntegrations.WorkerSchedulingApiUrl);
            }
        });

        // Register specific API clients
        services.AddScoped<IWorkerSchedulingApiClient, WorkerSchedulingApiClient>();

        return services;
    }
}

/// <summary>
/// Simple HTTP context accessor implementation for testing
/// </summary>
public class SimpleHttpContextAccessor : Microsoft.AspNetCore.Http.IHttpContextAccessor
{
    public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
}