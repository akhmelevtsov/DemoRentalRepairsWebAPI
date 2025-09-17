using Xunit;
using FluentAssertions;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Tests.Integration;

public class Step6ValidationTests
{
    [Fact]
    public void Step6_DomainLayer_CompleteValidation()
    {
        // This test validates that Step 6 is complete by testing all major domain components
        
        // ? 1. Test Value Objects
        var address = new PropertyAddress("123", "Main St", "Test City", "12345");
        address.Should().NotBeNull();
        address.FullAddress.Should().Be("123 Main St, Test City, 12345");

        var contact = new PersonContactInfo("John", "Doe", "john@test.com", "555-1234");
        contact.Should().NotBeNull();
        contact.GetFullName().Should().Be("John Doe");

        // ? 2. Test Property Aggregate Root
        var property = new Property(
            "Test Property",
            "TP001",
            address,
            "555-1234",
            contact,
            new List<string> { "101", "102", "103" },
            "noreply@test.com");

        property.Should().NotBeNull();
        property.DomainEvents.Should().HaveCount(1); // PropertyRegisteredEvent
        property.Name.Should().Be("Test Property");
        property.Code.Should().Be("TP001");

        // ? 3. Test Tenant Registration (Aggregate Interaction)
        var tenantContact = new PersonContactInfo("Jane", "Smith", "jane@test.com");
        var tenant = property.RegisterTenant(tenantContact, "101");

        tenant.Should().NotBeNull();
        tenant.UnitNumber.Should().Be("101");
        property.Tenants.Should().Contain(tenant);
        property.DomainEvents.Should().HaveCount(2); // PropertyRegistered + TenantRegistered

        // ? 4. Test TenantRequest Aggregate Root with State Machine
        var request = tenant.CreateRequest("Plumbing Issue", "Kitchen faucet leaking", "High");
        request.Should().NotBeNull();
        request.Status.Should().Be(TenantRequestStatus.Draft);
        request.Title.Should().Be("Plumbing Issue");
        request.UrgencyLevel.Should().Be("High");

        // Test status transitions
        request.Submit();
        request.Status.Should().Be(TenantRequestStatus.Submitted);

        var futureDate = DateTime.UtcNow.AddDays(1);
        request.Schedule(futureDate, "worker@test.com", "WO-001");
        request.Status.Should().Be(TenantRequestStatus.Scheduled);

        request.ReportWorkCompleted(true, "Fixed successfully");
        request.Status.Should().Be(TenantRequestStatus.Done);

        request.Close("All work completed");
        request.Status.Should().Be(TenantRequestStatus.Closed);

        // ? 5. Test Worker Entity
        var workerContact = new PersonContactInfo("Bob", "Builder", "bob@workers.com");
        var worker = new Worker(workerContact);

        worker.Should().NotBeNull();
        worker.IsActive.Should().BeTrue();
        worker.ContactInfo.GetFullName().Should().Be("Bob Builder");

        worker.SetSpecialization("Plumbing");
        worker.Specialization.Should().Be("Plumbing");

        // ? 6. Test Domain Services
        var validationService = new DomainValidationService();
        validationService.Should().NotBeNull();

        // Test validation doesn't throw for valid entities
        var validatePropertyAction = () => validationService.Validate(property);
        validatePropertyAction.Should().NotThrow();

        var validateTenantAction = () => validationService.Validate(tenant);
        validateTenantAction.Should().NotThrow();

        var validateWorkerAction = () => validationService.Validate(worker);
        validateWorkerAction.Should().NotThrow();

        // ? 7. Test Specifications
        var propertyByCodeSpec = new PropertyByCodeSpecification("TP001");
        propertyByCodeSpec.Should().NotBeNull();
        propertyByCodeSpec.IsSatisfiedBy(property).Should().BeTrue();

        var requestByStatusSpec = new TenantRequestByStatusSpecification(TenantRequestStatus.Closed);
        requestByStatusSpec.Should().NotBeNull();
        requestByStatusSpec.IsSatisfiedBy(request).Should().BeTrue();

        // ? 8. Test Domain Events Were Generated
        property.DomainEvents.Should().NotBeEmpty();
        request.DomainEvents.Should().NotBeEmpty();
        worker.DomainEvents.Should().NotBeEmpty();

        // ? 9. Test Business Rules
        var requestPrioritizationService = new RequestPrioritizationService(null!, null!);
        var priorityScore = requestPrioritizationService.CalculatePriorityScore(request);
        priorityScore.Should().BeGreaterThan(0);

        var workerAssignmentService = new WorkerAssignmentService(null!, null!);
        var specialization = workerAssignmentService.DetermineRequiredSpecialization(request);
        specialization.Should().Be("Plumbing");
    }

    [Fact]
    public void Step6_AllRepositoryInterfaces_Exist()
    {
        // Validate all repository interfaces are properly defined
        var repositoryTypes = new[]
        {
            typeof(Repositories.IPropertyRepository),
            typeof(Repositories.ITenantRepository),
            typeof(Repositories.ITenantRequestRepository),
            typeof(Repositories.IWorkerRepository)
        };

        foreach (var repoType in repositoryTypes)
        {
            repoType.Should().NotBeNull();
            repoType.IsInterface.Should().BeTrue();
        }
    }

    [Fact]
    public void Step6_AllSpecifications_WorkCorrectly()
    {
        // Test that all specifications compile and work
        var property = CreateTestProperty();
        var tenant = CreateTestTenant(property);
        var request = CreateTestRequest(tenant);
        var worker = CreateTestWorker();

        // Property specifications
        new PropertyWithTenantsSpecification().Should().NotBeNull();
        new PropertyByCodeSpecification("TP001").IsSatisfiedBy(property).Should().BeTrue();
        new PropertiesByCitySpecification("Test City").IsSatisfiedBy(property).Should().BeTrue();

        // Request specifications
        request.Submit(); // Move to submitted state
        new TenantRequestByStatusSpecification(TenantRequestStatus.Submitted).IsSatisfiedBy(request).Should().BeTrue();
        new PendingTenantRequestsSpecification().IsSatisfiedBy(request).Should().BeTrue();
        new TenantRequestsByUrgencySpecification("Normal").IsSatisfiedBy(request).Should().BeTrue();

        // Worker specifications
        new ActiveWorkersSpecification().IsSatisfiedBy(worker).Should().BeTrue();
        new WorkerByEmailSpecification("bob@workers.com").IsSatisfiedBy(worker).Should().BeTrue();
    }

    [Fact]
    public void Step6_DomainServices_AllFunctionProperly()
    {
        // Test all domain services can be instantiated and basic functions work
        var services = new[]
        {
            typeof(DomainValidationService),
            typeof(PropertyDomainService),
            typeof(TenantRequestDomainService),
            typeof(WorkerAssignmentService),
            typeof(RequestPrioritizationService),
            typeof(BusinessRulesEngine)
        };

        foreach (var serviceType in services)
        {
            serviceType.Should().NotBeNull();
            serviceType.IsClass.Should().BeTrue();
            serviceType.IsAbstract.Should().BeFalse();
        }

        // Test specific domain service functions
        var validationService = new DomainValidationService();
        var prioritizationService = new RequestPrioritizationService(null!, null!);
        var assignmentService = new WorkerAssignmentService(null!, null!);

        var testRequest = CreateTestRequest(CreateTestTenant(CreateTestProperty()));
        
        // Test priority calculation
        var score = prioritizationService.CalculatePriorityScore(testRequest);
        score.Should().BeGreaterThan(0);

        // Test safety detection
        var safetyRequest = CreateTestRequest(CreateTestTenant(CreateTestProperty()), "Gas leak emergency");
        prioritizationService.IsSafetyRelated(safetyRequest).Should().BeTrue();

        // Test worker specialization determination
        var plumbingRequest = CreateTestRequest(CreateTestTenant(CreateTestProperty()), "Plumbing issue");
        assignmentService.DetermineRequiredSpecialization(plumbingRequest).Should().Be("Plumbing");
    }

    // Helper methods for test data
    private static Property CreateTestProperty()
    {
        return new Property(
            "Test Property",
            "TP001",
            new PropertyAddress("123", "Test St", "Test City", "12345"),
            "555-1234",
            new PersonContactInfo("John", "Doe", "john@test.com"),
            new List<string> { "101", "102" },
            "noreply@test.com");
    }

    private static Tenant CreateTestTenant(Property property)
    {
        var contactInfo = new PersonContactInfo("Jane", "Smith", "jane@test.com");
        return property.RegisterTenant(contactInfo, "101");
    }

    private static TenantRequest CreateTestRequest(Tenant tenant, string title = "Test Request")
    {
        return tenant.CreateRequest(title, "Test Description", "Normal");
    }

    private static Worker CreateTestWorker()
    {
        var contactInfo = new PersonContactInfo("Bob", "Builder", "bob@workers.com");
        return new Worker(contactInfo);
    }
}