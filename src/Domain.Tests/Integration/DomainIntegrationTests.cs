using Xunit;
using FluentAssertions;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Services;

namespace RentalRepairs.Domain.Tests.Integration;

public class DomainIntegrationTests
{
    [Fact]
    public void CanCreateCompletePropertyWorkflow()
    {
        // Test that we can create a complete property workflow without compilation errors
        
        // Arrange - Create property
        var address = new PropertyAddress("123", "Main St", "Test City", "12345");
        var superintendent = new PersonContactInfo("John", "Doe", "john@test.com");
        var property = new Property(
            "Test Property",
            "TP001", 
            address,
            "555-1234",
            superintendent,
            new List<string> { "101", "102", "103" },
            "noreply@testproperty.com");

        // Act & Assert - Register tenant
        var tenantContact = new PersonContactInfo("Jane", "Smith", "jane@test.com");
        var tenant = property.RegisterTenant(tenantContact, "101");
        
        tenant.Should().NotBeNull();
        tenant.UnitNumber.Should().Be("101");
        property.Tenants.Should().Contain(tenant);

        // Act & Assert - Create tenant request
        var request = tenant.CreateRequest("Leaky faucet", "Kitchen faucet is leaking", "Normal");
        
        request.Should().NotBeNull();
        request.Title.Should().Be("Leaky faucet");
        request.Code.Should().StartWith("TP001-101-");
        
        // Act & Assert - Request workflow
        request.Submit();
        request.Status.Should().Be(Enums.TenantRequestStatus.Submitted);
        
        var futureDate = DateTime.UtcNow.AddDays(1);
        request.Schedule(futureDate, "worker@test.com", "WO-001");
        request.Status.Should().Be(Enums.TenantRequestStatus.Scheduled);
        
        request.ReportWorkCompleted(true, "Work completed successfully");
        request.Status.Should().Be(Enums.TenantRequestStatus.Done);
        
        request.Close("All work completed");
        request.Status.Should().Be(Enums.TenantRequestStatus.Closed);

        // Verify domain events were created
        property.DomainEvents.Should().NotBeEmpty();
        request.DomainEvents.Should().NotBeEmpty();
    }

    [Fact]
    public void CanCreateWorkerAndValidateServices()
    {
        // Test worker creation and domain services
        
        // Arrange
        var workerContact = new PersonContactInfo("Bob", "Builder", "bob@workers.com");
        var worker = new Worker(workerContact);
        
        // Act & Assert
        worker.Should().NotBeNull();
        worker.IsActive.Should().BeTrue();
        worker.ContactInfo.GetFullName().Should().Be("Bob Builder");
        
        // Test worker specialization
        worker.SetSpecialization("Plumbing");
        worker.Specialization.Should().Be("Plumbing");
        
        // Test worker notes
        worker.AddNotes("Experienced plumber");
        worker.Notes.Should().Contain("Experienced plumber");
        
        // Test domain services can be instantiated
        var validationService = new DomainValidationService();
        validationService.Should().NotBeNull();
        
        // Test that worker validates
        var validationAction = () => validationService.Validate(worker);
        validationAction.Should().NotThrow();
    }

    [Fact]
    public void ValueObjectsWorkCorrectly()
    {
        // Test value objects equality and immutability
        
        // Arrange
        var address1 = new PropertyAddress("123", "Main St", "City", "12345");
        var address2 = new PropertyAddress("123", "Main St", "City", "12345");
        var address3 = new PropertyAddress("456", "Oak St", "City", "67890");
        
        // Act & Assert
        address1.Should().Be(address2); // Value equality
        address1.Should().NotBe(address3);
        address1.GetHashCode().Should().Be(address2.GetHashCode());
        
        // Test PersonContactInfo
        var contact1 = new PersonContactInfo("John", "Doe", "john@test.com");
        var contact2 = new PersonContactInfo("John", "Doe", "john@test.com");
        
        contact1.Should().Be(contact2);
        contact1.GetFullName().Should().Be("John Doe");
    }
}

// Simple test to verify compilation of domain services
public class DomainServicesCompilationTest
{
    [Fact]
    public void DomainServicesCanBeInstantiated()
    {
        // Test that all domain services can be instantiated (compilation test)
        var validationService = new DomainValidationService();
        validationService.Should().NotBeNull();
        
        // Note: Other services require dependencies, so we just test they exist
        var serviceTypes = new[]
        {
            typeof(PropertyDomainService),
            typeof(TenantRequestDomainService), 
            typeof(WorkerAssignmentService),
            typeof(RequestPrioritizationService)
        };
        
        foreach (var serviceType in serviceTypes)
        {
            serviceType.Should().NotBeNull();
            serviceType.IsClass.Should().BeTrue();
        }
    }
}