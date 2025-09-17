using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Enums;
using Xunit;
using FluentAssertions;

namespace RentalRepairs.Domain.Tests.Services;

public class RequestPrioritizationServiceTests
{
    private readonly RequestPrioritizationService _service;

    public RequestPrioritizationServiceTests()
    {
        // For unit tests, we can create a service without dependencies for testing pure functions
        _service = new RequestPrioritizationService(null!, null!);
    }

    [Theory]
    [InlineData("Critical", 100)]
    [InlineData("High", 75)]
    [InlineData("Normal", 50)]
    [InlineData("Low", 25)]
    public void CalculatePriorityScore_ShouldAssignCorrectBaseScore_BasedOnUrgency(string urgency, int expectedBaseScore)
    {
        // Arrange
        var request = CreateTestRequest(urgency: urgency);

        // Act
        var score = _service.CalculatePriorityScore(request);

        // Assert
        score.Should().BeGreaterOrEqualTo(expectedBaseScore);
    }

    [Fact]
    public void CalculatePriorityScore_ShouldAddAgeBonus_ForOlderRequests()
    {
        // Arrange
        var oldRequest = CreateTestRequest(createdDaysAgo: 10);
        var newRequest = CreateTestRequest(createdDaysAgo: 1);

        // Act
        var oldScore = _service.CalculatePriorityScore(oldRequest);
        var newScore = _service.CalculatePriorityScore(newRequest);

        // Assert
        oldScore.Should().BeGreaterThan(newScore);
    }

    [Fact]
    public void IsSafetyRelated_ShouldReturnTrue_ForSafetyKeywords()
    {
        // Arrange
        var safetyRequest = CreateTestRequest(title: "Gas leak emergency", description: "Dangerous gas leak in kitchen");

        // Act
        var result = _service.IsSafetyRelated(safetyRequest);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmergency_ShouldReturnTrue_ForEmergencyKeywords()
    {
        // Arrange
        var emergencyRequest = CreateTestRequest(title: "Urgent flooding", description: "Immediate attention needed");

        // Act
        var result = _service.IsEmergency(emergencyRequest);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CalculatePriorityScore_ShouldAddSafetyBonus_ForSafetyRelatedRequests()
    {
        // Arrange
        var safetyRequest = CreateTestRequest(title: "Electrical hazard", description: "Exposed wires");
        var normalRequest = CreateTestRequest(title: "Paint touch up", description: "Minor cosmetic issue");

        // Act
        var safetyScore = _service.CalculatePriorityScore(safetyRequest);
        var normalScore = _service.CalculatePriorityScore(normalRequest);

        // Assert
        safetyScore.Should().BeGreaterThan(normalScore);
    }

    [Fact]
    public void CalculatePriorityScore_ShouldAddRetryBonus_ForMultipleServiceAttempts()
    {
        // Arrange
        var request = CreateTestRequest();
        
        // Simulate multiple service attempts
        typeof(TenantRequest)
            .GetProperty(nameof(TenantRequest.ServiceWorkOrderCount))!
            .SetValue(request, 3);

        // Act
        var score = _service.CalculatePriorityScore(request);

        // Assert
        score.Should().BeGreaterThan(50); // Base score + retry bonus
    }

    private static TenantRequest CreateTestRequest(
        string title = "Test Request",
        string description = "Test Description",
        string urgency = "Normal",
        int createdDaysAgo = 0)
    {
        var property = CreateTestProperty();
        var tenant = CreateTestTenant(property);
        var request = tenant.CreateRequest(title, description, urgency);

        // Set creation date
        if (createdDaysAgo > 0)
        {
            var createdDate = DateTime.UtcNow.AddDays(-createdDaysAgo);
            typeof(TenantRequest)
                .GetProperty(nameof(TenantRequest.CreatedAt))!
                .SetValue(request, createdDate);
        }

        return request;
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
}