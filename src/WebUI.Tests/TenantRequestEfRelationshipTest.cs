using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Infrastructure.Persistence;
using RentalRepairs.WebUI.Tests.Integration;
using Xunit;

namespace RentalRepairs.WebUI.Tests;

/// <summary>
/// Test to validate that the TenantRequest to Tenant relationship is correctly configured
/// </summary>
public class TenantRequestEfRelationshipTest : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;

    public TenantRequestEfRelationshipTest(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TenantRequest_To_Tenant_Relationship_Should_Work_Correctly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Act & Assert - This should not throw an exception if the relationship is configured correctly
        await context.Database.EnsureCreatedAsync();
        
        // Additional verification - create entities and test the relationship
        var property = new Property(
            "Test Property",
            "TP-001",
            new PropertyAddress("123", "Test St", "Test City", "12345"),
            "555-1234",
            new PersonContactInfo("John", "Doe", "john@test.com"),
            new List<string> { "101", "102" },
            "noreply@test.com");

        var tenant = property.RegisterTenant(
            new PersonContactInfo("Jane", "Smith", "jane@test.com"),
            "101");

        var tenantRequest = tenant.CreateRequest("Test Request", "Test Description", "Normal");

        // Verify the foreign key is set correctly
        tenantRequest.TenantId.Should().Be(tenant.Id);
        tenantRequest.Tenant.Should().Be(tenant);
        
        // Verify the computed property still works
        tenantRequest.TenantIdentifier.Should().Be(tenant.Id.ToString());
    }
}