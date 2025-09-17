using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Enums;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RentalRepairs.Domain.Tests.Specifications;

public class TenantRequestSpecificationsTests
{
    [Fact]
    public void TenantRequestByStatusSpecification_ShouldFilterByStatus()
    {
        // Arrange
        var property = CreateTestProperty();
        var tenant = CreateTestTenant(property);
        
        var submittedRequest = CreateTestTenantRequest(tenant, "REQ001");
        submittedRequest.Submit();
        
        var scheduledRequest = CreateTestTenantRequest(tenant, "REQ002");
        scheduledRequest.Submit();
        scheduledRequest.Schedule(DateTime.UtcNow.AddDays(1), "worker@test.com", "WO001");

        var requests = new List<TenantRequest> { submittedRequest, scheduledRequest };

        var specification = new TenantRequestByStatusSpecification(TenantRequestStatus.Submitted);

        // Act
        var filteredRequests = requests.Where(r => specification.IsSatisfiedBy(r)).ToList();

        // Assert
        filteredRequests.Should().HaveCount(1);
        filteredRequests.First().Status.Should().Be(TenantRequestStatus.Submitted);
    }

    [Fact]
    public void PendingTenantRequestsSpecification_ShouldFilterPendingRequests()
    {
        // Arrange
        var property = CreateTestProperty();
        var tenant = CreateTestTenant(property);
        
        var submittedRequest = CreateTestTenantRequest(tenant, "REQ001");
        submittedRequest.Submit();
        
        var scheduledRequest = CreateTestTenantRequest(tenant, "REQ002");
        scheduledRequest.Submit();
        scheduledRequest.Schedule(DateTime.UtcNow.AddDays(1), "worker@test.com", "WO001");
        
        var closedRequest = CreateTestTenantRequest(tenant, "REQ003");
        closedRequest.Submit();
        // Follow proper workflow: Submitted -> Scheduled -> Done -> Closed
        closedRequest.Schedule(DateTime.UtcNow.AddDays(1), "worker@test.com", "WO002");
        closedRequest.ReportWorkCompleted(true, "Work completed successfully");
        closedRequest.Close("Request completed");

        var requests = new List<TenantRequest> { submittedRequest, scheduledRequest, closedRequest };

        var specification = new PendingTenantRequestsSpecification();

        // Act
        var pendingRequests = requests.Where(r => specification.IsSatisfiedBy(r)).ToList();

        // Assert
        pendingRequests.Should().HaveCount(2);
        pendingRequests.Should().Contain(r => r.Status == TenantRequestStatus.Submitted);
        pendingRequests.Should().Contain(r => r.Status == TenantRequestStatus.Scheduled);
    }

    [Fact]
    public void TenantRequestsByUrgencySpecification_ShouldFilterByUrgencyLevel()
    {
        // Arrange
        var property = CreateTestProperty();
        var tenant = CreateTestTenant(property);
        
        var highPriorityRequest = CreateTestTenantRequest(tenant, "REQ001", urgencyLevel: "High");
        highPriorityRequest.Submit();
        
        var normalPriorityRequest = CreateTestTenantRequest(tenant, "REQ002", urgencyLevel: "Normal");
        normalPriorityRequest.Submit();

        var requests = new List<TenantRequest> { highPriorityRequest, normalPriorityRequest };

        var specification = new TenantRequestsByUrgencySpecification("High");

        // Act
        var filteredRequests = requests.Where(r => specification.IsSatisfiedBy(r)).ToList();

        // Assert
        filteredRequests.Should().HaveCount(1);
        filteredRequests.First().UrgencyLevel.Should().Be("High");
    }

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

    private static TenantRequest CreateTestTenantRequest(Tenant tenant, string code, string urgencyLevel = "Normal")
    {
        return tenant.CreateRequest("Test Request", "Test Description", urgencyLevel);
    }
}