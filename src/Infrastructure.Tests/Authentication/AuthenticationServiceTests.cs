using FluentAssertions;
using Microsoft.Extensions.Logging;
using RentalRepairs.Infrastructure.Authentication;
using RentalRepairs.Infrastructure.Tests.Services;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Authentication;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_Should_Return_Success_For_Valid_Credentials()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<AuthenticationService>();
        var authService = new AuthenticationService(null!, null!, null!, logger);

        // Act
        var result = await authService.AuthenticateAsync("admin@test.com", "password123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Email.Should().Be("admin@test.com");
        result.UserId.Should().Be("admin@test.com");
        result.Roles.Should().Contain(UserRoles.SystemAdmin);
        result.Token.Should().NotBeEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_Failure_For_Empty_Credentials()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<AuthenticationService>();
        var authService = new AuthenticationService(null!, null!, null!, logger);

        // Act
        var result = await authService.AuthenticateAsync("", "");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Email and password are required");
    }

    [Fact]
    public async Task ValidateTokenAsync_Should_Return_True_For_Valid_Token()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<AuthenticationService>();
        var authService = new AuthenticationService(null!, null!, null!, logger);

        // Act
        var result = await authService.ValidateTokenAsync("token_valid_12345");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_Should_Return_False_For_Invalid_Token()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<AuthenticationService>();
        var authService = new AuthenticationService(null!, null!, null!, logger);

        // Act
        var result = await authService.ValidateTokenAsync("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UserRoles_Should_Have_Required_Constants()
    {
        // Assert
        UserRoles.SystemAdmin.Should().Be("SystemAdmin");
        UserRoles.PropertySuperintendent.Should().Be("PropertySuperintendent");
        UserRoles.Tenant.Should().Be("Tenant");
        UserRoles.Worker.Should().Be("Worker");
    }

    [Fact]
    public void CustomClaims_Should_Have_Required_Constants()
    {
        // Assert
        CustomClaims.PropertyId.Should().Be("property_id");
        CustomClaims.UnitNumber.Should().Be("unit_number");
        CustomClaims.TenantId.Should().Be("tenant_id");
        CustomClaims.WorkerSpecialization.Should().Be("worker_specialization");
        CustomClaims.WorkerId.Should().Be("worker_id");
        CustomClaims.IsActive.Should().Be("is_active");
    }

    [Fact]
    public void AuthenticationResult_Should_Have_Default_Values()
    {
        // Arrange & Act
        var result = new AuthenticationResult();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.UserId.Should().BeEmpty();
        result.Email.Should().BeEmpty();
        result.DisplayName.Should().BeEmpty();
        result.Token.Should().BeEmpty();
        result.Roles.Should().NotBeNull().And.BeEmpty();
        result.Claims.Should().NotBeNull().And.BeEmpty();
    }
}