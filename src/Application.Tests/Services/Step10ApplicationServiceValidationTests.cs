using FluentAssertions;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Application.Services;
using Xunit;

namespace RentalRepairs.Application.Tests.Services;

public class Step10ApplicationServiceValidationTests
{
    [Fact]
    public void All_Required_Application_Services_Exist()
    {
        // This test validates that Step 10 application services are implemented
        
        // Arrange & Act - Check that all required types exist per migration plan
        var propertyServiceType = typeof(PropertyService);
        var tenantRequestServiceType = typeof(TenantRequestService);
        var workerServiceType = typeof(WorkerService);
        var notifyPartiesServiceType = typeof(NotifyPartiesService);
        
        // Required interfaces per migration plan
        var iPropertyServiceType = typeof(IPropertyService);
        var iTenantRequestServiceType = typeof(ITenantRequestService);
        var iWorkerServiceType = typeof(IWorkerService);
        var iNotifyPartiesServiceType = typeof(INotifyPartiesService);

        // Assert - All services and interfaces exist
        propertyServiceType.Should().NotBeNull();
        tenantRequestServiceType.Should().NotBeNull();
        workerServiceType.Should().NotBeNull();
        notifyPartiesServiceType.Should().NotBeNull();
        
        iPropertyServiceType.Should().NotBeNull();
        iTenantRequestServiceType.Should().NotBeNull();
        iWorkerServiceType.Should().NotBeNull();
        iNotifyPartiesServiceType.Should().NotBeNull();

        // Verify implementations match interfaces
        propertyServiceType.Should().BeAssignableTo<IPropertyService>();
        tenantRequestServiceType.Should().BeAssignableTo<ITenantRequestService>();
        workerServiceType.Should().BeAssignableTo<IWorkerService>();
        notifyPartiesServiceType.Should().BeAssignableTo<INotifyPartiesService>();
    }

    [Fact]
    public void Application_Service_Interfaces_Have_Required_Methods()
    {
        // Test that application service interfaces have the expected methods for business operations
        
        var propertyServiceMethods = typeof(IPropertyService).GetMethods().Select(m => m.Name).ToList();
        var tenantRequestServiceMethods = typeof(ITenantRequestService).GetMethods().Select(m => m.Name).ToList();
        var workerServiceMethods = typeof(IWorkerService).GetMethods().Select(m => m.Name).ToList();
        var notifyPartiesServiceMethods = typeof(INotifyPartiesService).GetMethods().Select(m => m.Name).ToList();

        // Assert IPropertyService has required methods
        propertyServiceMethods.Should().Contain("RegisterPropertyAsync");
        propertyServiceMethods.Should().Contain("GetPropertyByIdAsync");
        propertyServiceMethods.Should().Contain("GetPropertyByCodeAsync");
        propertyServiceMethods.Should().Contain("GetPropertiesAsync");
        propertyServiceMethods.Should().Contain("RegisterTenantAsync");
        propertyServiceMethods.Should().Contain("IsUnitAvailableAsync");

        // Assert ITenantRequestService has required methods
        tenantRequestServiceMethods.Should().Contain("CreateTenantRequestAsync");
        tenantRequestServiceMethods.Should().Contain("SubmitTenantRequestAsync");
        tenantRequestServiceMethods.Should().Contain("ScheduleServiceWorkAsync");
        tenantRequestServiceMethods.Should().Contain("ReportWorkCompletedAsync");
        tenantRequestServiceMethods.Should().Contain("CloseRequestAsync");
        tenantRequestServiceMethods.Should().Contain("GetTenantRequestByIdAsync");

        // Assert IWorkerService has required methods
        workerServiceMethods.Should().Contain("RegisterWorkerAsync");
        workerServiceMethods.Should().Contain("GetWorkerByIdAsync");
        workerServiceMethods.Should().Contain("GetWorkerByEmailAsync");
        workerServiceMethods.Should().Contain("GetAvailableWorkersAsync");
        workerServiceMethods.Should().Contain("IsWorkerAvailableAsync");

        // Assert INotifyPartiesService has required methods
        notifyPartiesServiceMethods.Should().Contain("NotifyTenantRequestSubmittedAsync");
        notifyPartiesServiceMethods.Should().Contain("NotifyTenantWorkScheduledAsync");
        notifyPartiesServiceMethods.Should().Contain("NotifySuperintendentNewRequestAsync");
        notifyPartiesServiceMethods.Should().Contain("NotifyWorkerWorkScheduledAsync");
        notifyPartiesServiceMethods.Should().Contain("SendCustomNotificationAsync");
    }

    [Fact]
    public void Application_Services_Have_Public_Constructors()
    {
        // Verify that all application services can be instantiated (for dependency injection)
        
        var propertyServiceType = typeof(PropertyService);
        var tenantRequestServiceType = typeof(TenantRequestService);
        var workerServiceType = typeof(WorkerService);
        var notifyPartiesServiceType = typeof(NotifyPartiesService);

        // Assert that all have public constructors with dependencies
        propertyServiceType.GetConstructors().Should().HaveCountGreaterThan(0);
        tenantRequestServiceType.GetConstructors().Should().HaveCountGreaterThan(0);
        workerServiceType.GetConstructors().Should().HaveCountGreaterThan(0);
        notifyPartiesServiceType.GetConstructors().Should().HaveCountGreaterThan(0);

        // Verify constructors take expected dependencies
        var propertyServiceConstructor = propertyServiceType.GetConstructors().First();
        var tenantRequestServiceConstructor = tenantRequestServiceType.GetConstructors().First();
        var workerServiceConstructor = workerServiceType.GetConstructors().First();

        propertyServiceConstructor.GetParameters().Should().HaveCount(1); // IMediator
        tenantRequestServiceConstructor.GetParameters().Should().HaveCount(1); // IMediator
        workerServiceConstructor.GetParameters().Should().HaveCount(1); // IMediator
    }

    [Fact]
    public void Step10_Success_Criteria_Met()
    {
        // Validate that Step 10 success criteria from the migration plan are met
        
        // ? IPropertyService interface moved to src/Application/Interfaces/
        var iPropertyServiceType = typeof(IPropertyService);
        iPropertyServiceType.Should().NotBeNull();
        iPropertyServiceType.Namespace.Should().Be("RentalRepairs.Application.Interfaces");

        // ? INotifyPartiesService interface moved to src/Application/Interfaces/
        var iNotifyPartiesServiceType = typeof(INotifyPartiesService);
        iNotifyPartiesServiceType.Should().NotBeNull();
        iNotifyPartiesServiceType.Namespace.Should().Be("RentalRepairs.Application.Interfaces");

        // ? Application-specific DTOs created (already exist from previous steps)
        var propertyDtoType = typeof(Application.DTOs.PropertyDto);
        var tenantRequestDtoType = typeof(Application.DTOs.TenantRequestDto);
        propertyDtoType.Should().NotBeNull();
        tenantRequestDtoType.Should().NotBeNull();

        // ? Mapping configurations created (updated to use Mapster)
        var mappingConfigType = typeof(Application.Mappings.DomainToResponseMappingConfig);
        mappingConfigType.Should().NotBeNull();

        // ? FluentValidation implemented (already implemented in previous steps)
        var validatorType = typeof(Application.Validators.Properties.RegisterPropertyCommandValidator);
        validatorType.Should().NotBeNull();

        // ? Application service implementations
        var propertyServiceType = typeof(PropertyService);
        var tenantRequestServiceType = typeof(TenantRequestService);
        var workerServiceType = typeof(WorkerService);
        var notifyPartiesServiceType = typeof(NotifyPartiesService);

        propertyServiceType.Should().NotBeNull();
        tenantRequestServiceType.Should().NotBeNull();
        workerServiceType.Should().NotBeNull();
        notifyPartiesServiceType.Should().NotBeNull();
    }

    [Fact]
    public void Application_Services_Use_CQRS_Pattern()
    {
        // Verify that application services properly orchestrate CQRS operations
        
        // Application services should depend on IMediator for CQRS
        var propertyServiceConstructor = typeof(PropertyService).GetConstructors().First();
        var tenantRequestServiceConstructor = typeof(TenantRequestService).GetConstructors().First();
        var workerServiceConstructor = typeof(WorkerService).GetConstructors().First();

        // Check that services take IMediator as dependency
        propertyServiceConstructor.GetParameters()
            .Should().Contain(p => p.ParameterType.Name == "IMediator");
        tenantRequestServiceConstructor.GetParameters()
            .Should().Contain(p => p.ParameterType.Name == "IMediator");
        workerServiceConstructor.GetParameters()
            .Should().Contain(p => p.ParameterType.Name == "IMediator");

        // Services should be in correct namespace
        typeof(PropertyService).Namespace.Should().Be("RentalRepairs.Application.Services");
        typeof(TenantRequestService).Namespace.Should().Be("RentalRepairs.Application.Services");
        typeof(WorkerService).Namespace.Should().Be("RentalRepairs.Application.Services");
        typeof(NotifyPartiesService).Namespace.Should().Be("RentalRepairs.Application.Services");
    }
}