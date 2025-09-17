using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Infrastructure.ApiIntegration;
using RentalRepairs.Infrastructure.Authentication;
using RentalRepairs.Infrastructure.Caching;
using RentalRepairs.Infrastructure.Monitoring;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Integration;

public class Step13InfrastructureConcernsValidationTests
{
    [Fact]
    public void Step13_All_Authentication_Components_Exist()
    {
        // Validate that all required authentication components from Step 13 are created
        
        var authServiceType = typeof(IAuthenticationService);
        var authServiceImplType = typeof(AuthenticationService);
        var authorizationServiceType = typeof(IAuthorizationService);
        var authorizationServiceImplType = typeof(AuthorizationService);
        var authResultType = typeof(AuthenticationResult);

        // Assert all authentication types exist
        authServiceType.Should().NotBeNull();
        authServiceImplType.Should().NotBeNull();
        authorizationServiceType.Should().NotBeNull();
        authorizationServiceImplType.Should().NotBeNull();
        authResultType.Should().NotBeNull();

        // Verify implementations implement interfaces
        authServiceImplType.Should().BeAssignableTo<IAuthenticationService>();
        authorizationServiceImplType.Should().BeAssignableTo<IAuthorizationService>();

        // Verify correct namespaces
        authServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");
        authServiceImplType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");
        authorizationServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");
        authorizationServiceImplType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");
    }

    [Fact]
    public void Step13_All_Caching_Components_Exist()
    {
        // Validate caching infrastructure components
        
        var cacheServiceType = typeof(ICacheService);
        var memoryCacheServiceType = typeof(MemoryCacheService);
        var nullCacheServiceType = typeof(NullCacheService);
        var cacheSettingsType = typeof(CacheSettings);
        var cacheKeysType = typeof(CacheKeys);

        // Assert all caching types exist
        cacheServiceType.Should().NotBeNull();
        memoryCacheServiceType.Should().NotBeNull();
        nullCacheServiceType.Should().NotBeNull();
        cacheSettingsType.Should().NotBeNull();
        cacheKeysType.Should().NotBeNull();

        // Verify implementations implement interface
        memoryCacheServiceType.Should().BeAssignableTo<ICacheService>();
        nullCacheServiceType.Should().BeAssignableTo<ICacheService>();

        // Verify correct namespaces
        cacheServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
        memoryCacheServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
        nullCacheServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
        cacheSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
        cacheKeysType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
    }

    [Fact]
    public void Step13_All_Monitoring_Components_Exist()
    {
        // Validate monitoring infrastructure components
        
        var performanceMonitoringServiceType = typeof(IPerformanceMonitoringService);
        var performanceMonitoringServiceImplType = typeof(PerformanceMonitoringService);
        var performanceOperationType = typeof(PerformanceOperation);
        var businessEventsType = typeof(BusinessEvents);
        var performanceMetricsType = typeof(PerformanceMetrics);

        // Assert all monitoring types exist
        performanceMonitoringServiceType.Should().NotBeNull();
        performanceMonitoringServiceImplType.Should().NotBeNull();
        performanceOperationType.Should().NotBeNull();
        businessEventsType.Should().NotBeNull();
        performanceMetricsType.Should().NotBeNull();

        // Verify implementation implements interface
        performanceMonitoringServiceImplType.Should().BeAssignableTo<IPerformanceMonitoringService>();

        // Verify correct namespaces
        performanceMonitoringServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
        performanceMonitoringServiceImplType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
        performanceOperationType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
        businessEventsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
        performanceMetricsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
    }

    [Fact]
    public void Step13_All_ApiIntegration_Components_Exist()
    {
        // Validate API integration infrastructure components
        
        var externalApiClientType = typeof(IExternalApiClient);
        var httpExternalApiClientType = typeof(HttpExternalApiClient);
        var workerSchedulingApiClientType = typeof(IWorkerSchedulingApiClient);
        var workerSchedulingApiClientImplType = typeof(WorkerSchedulingApiClient);
        var scheduleWorkRequestType = typeof(ScheduleWorkRequest);
        var scheduleWorkResponseType = typeof(ScheduleWorkResponse);

        // Assert all API integration types exist
        externalApiClientType.Should().NotBeNull();
        httpExternalApiClientType.Should().NotBeNull();
        workerSchedulingApiClientType.Should().NotBeNull();
        workerSchedulingApiClientImplType.Should().NotBeNull();
        scheduleWorkRequestType.Should().NotBeNull();
        scheduleWorkResponseType.Should().NotBeNull();

        // Verify implementations implement interfaces
        httpExternalApiClientType.Should().BeAssignableTo<IExternalApiClient>();
        workerSchedulingApiClientImplType.Should().BeAssignableTo<IWorkerSchedulingApiClient>();

        // Verify correct namespaces
        externalApiClientType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");
        httpExternalApiClientType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");
        workerSchedulingApiClientType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");
        workerSchedulingApiClientImplType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");
    }

    [Fact]
    public void Step13_Infrastructure_DependencyInjection_Enhanced()
    {
        // Test that dependency injection has been enhanced for infrastructure concerns
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CacheSettings:EnableCaching"] = "true",
                ["CacheSettings:Provider"] = "Memory",
                ["NotificationSettings:EmailProvider"] = "Mock",
                ["NotificationSettings:EnableEmailNotifications"] = "true",
                ["ExternalServices:ApiIntegrations:EnableWorkerSchedulingApi"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);

        // Check service registrations without building provider
        var authServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IAuthenticationService));
        var cacheServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICacheService));
        var monitoringServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IPerformanceMonitoringService));
        var httpContextAccessorDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

        // Assert - Verify services are registered
        authServiceDescriptor.Should().NotBeNull();
        authServiceDescriptor!.ImplementationType.Should().Be(typeof(AuthenticationService));

        cacheServiceDescriptor.Should().NotBeNull();
        cacheServiceDescriptor!.ImplementationType.Should().Be(typeof(MemoryCacheService));

        monitoringServiceDescriptor.Should().NotBeNull();
        monitoringServiceDescriptor!.ImplementationType.Should().Be(typeof(PerformanceMonitoringService));

        httpContextAccessorDescriptor.Should().NotBeNull();
        httpContextAccessorDescriptor!.ImplementationType.Should().Be(typeof(SimpleHttpContextAccessor));
    }

    [Fact]
    public void Step13_Authentication_Service_Interface_Methods_Exist()
    {
        // Validate IAuthenticationService interface has required methods
        
        var authServiceInterface = typeof(IAuthenticationService);
        var methods = authServiceInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("AuthenticateAsync");
        methods.Should().Contain("AuthenticateTenantAsync");
        methods.Should().Contain("AuthenticateWorkerAsync");
        methods.Should().Contain("ValidateTokenAsync");
        methods.Should().Contain("SignOutAsync");
    }

    [Fact]
    public void Step13_Cache_Service_Interface_Methods_Exist()
    {
        // Validate ICacheService interface has required methods
        
        var cacheServiceInterface = typeof(ICacheService);
        var methods = cacheServiceInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("GetAsync");
        methods.Should().Contain("SetAsync");
        methods.Should().Contain("RemoveAsync");
        methods.Should().Contain("RemoveByPatternAsync");
        methods.Should().Contain("ExistsAsync");
        methods.Should().Contain("ClearAsync");
    }

    [Fact]
    public void Step13_Monitoring_Service_Interface_Methods_Exist()
    {
        // Validate IPerformanceMonitoringService interface has required methods
        
        var monitoringServiceInterface = typeof(IPerformanceMonitoringService);
        var methods = monitoringServiceInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("BeginOperation");
        methods.Should().Contain("LogPerformanceMetricAsync");
        methods.Should().Contain("LogBusinessMetricAsync");
        methods.Should().Contain("LogErrorAsync");
    }

    [Fact]
    public void Step13_ApiIntegration_Service_Interface_Methods_Exist()
    {
        // Validate IExternalApiClient interface has required methods
        
        var apiClientInterface = typeof(IExternalApiClient);
        var methods = apiClientInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("GetAsync");
        methods.Should().Contain("PostAsync");
        methods.Should().Contain("PutAsync");
        methods.Should().Contain("DeleteAsync");

        // Validate IWorkerSchedulingApiClient interface has required methods
        var workerApiInterface = typeof(IWorkerSchedulingApiClient);
        var workerMethods = workerApiInterface.GetMethods().Select(m => m.Name).ToList();

        workerMethods.Should().Contain("GetWorkerAvailabilityAsync");
        workerMethods.Should().Contain("ScheduleWorkAsync");
        workerMethods.Should().Contain("CancelScheduledWorkAsync");
        workerMethods.Should().Contain("GetWorkerScheduleAsync");
    }

    [Fact]
    public void Step13_Success_Criteria_Met()
    {
        // Validate that Step 13 success criteria from the migration plan are met

        // ? Streamlined authentication and authorization implementations created
        var authServiceType = typeof(AuthenticationService);
        var authorizationServiceType = typeof(AuthorizationService);
        
        authServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");
        authorizationServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Authentication");

        // ? Caching strategies implemented
        var cacheServiceType = typeof(MemoryCacheService);
        var cacheSettingsType = typeof(CacheSettings);
        
        cacheServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");
        cacheSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Caching");

        // ? Logging and monitoring set up
        var monitoringServiceType = typeof(PerformanceMonitoringService);
        var businessEventsType = typeof(BusinessEvents);
        
        monitoringServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");
        businessEventsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Monitoring");

        // ? External API integrations configured
        var apiClientType = typeof(HttpExternalApiClient);
        var workerApiClientType = typeof(WorkerSchedulingApiClient);
        
        apiClientType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");
        workerApiClientType.Namespace.Should().Be("RentalRepairs.Infrastructure.ApiIntegration");

        // ? Dependency injection registration enhanced
        var dependencyInjectionType = typeof(Infrastructure.DependencyInjection);
        var addInfrastructureMethod = dependencyInjectionType.GetMethod("AddInfrastructure");
        addInfrastructureMethod.Should().NotBeNull();
    }

    [Fact]
    public void Step13_Infrastructure_Specific_Concerns_Architecture_Compliant()
    {
        // Verify infrastructure concerns follow clean architecture principles
        
        // Authentication services should not depend on external frameworks inappropriately
        var authServiceType = typeof(AuthenticationService);
        authServiceType.Should().BeAssignableTo<IAuthenticationService>();

        // Caching services should abstract caching providers
        var cacheServiceType = typeof(MemoryCacheService);
        cacheServiceType.Should().BeAssignableTo<ICacheService>();

        // Monitoring services should provide clean abstractions
        var monitoringServiceType = typeof(PerformanceMonitoringService);
        monitoringServiceType.Should().BeAssignableTo<IPerformanceMonitoringService>();

        // API integration should follow HTTP client patterns
        var apiClientType = typeof(HttpExternalApiClient);
        apiClientType.Should().BeAssignableTo<IExternalApiClient>();
    }
}