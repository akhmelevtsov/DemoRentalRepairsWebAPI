using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Exceptions;
using Moq;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RentalRepairs.Domain.Tests.Services;

public class PropertyDomainServiceTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<DomainValidationService> _validationServiceMock;
    private readonly PropertyDomainService _propertyDomainService;

    public PropertyDomainServiceTests()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _validationServiceMock = new Mock<DomainValidationService>();
        _propertyDomainService = new PropertyDomainService(
            _propertyRepositoryMock.Object, 
            _tenantRepositoryMock.Object, 
            _validationServiceMock.Object);
    }

    [Fact]
    public async Task IsPropertyCodeUniqueAsync_ShouldReturnTrue_WhenCodeDoesNotExist()
    {
        // Arrange
        var code = "TEST001";
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);

        // Act
        var result = await _propertyDomainService.IsPropertyCodeUniqueAsync(code);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPropertyCodeUniqueAsync_ShouldReturnFalse_WhenCodeExists()
    {
        // Arrange
        var code = "TEST001";
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true);

        // Act
        var result = await _propertyDomainService.IsPropertyCodeUniqueAsync(code);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidatePropertyRegistrationAsync_ShouldThrowException_WhenCodeAlreadyExists()
    {
        // Arrange
        var code = "TEST001";
        var units = new List<string> { "101", "102" };
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PropertyDomainException>(
            () => _propertyDomainService.ValidatePropertyRegistrationAsync(code, units));
        
        exception.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task ValidatePropertyRegistrationAsync_ShouldThrowException_WhenUnitsAreEmpty()
    {
        // Arrange
        var code = "TEST001";
        var units = new List<string>();
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PropertyDomainException>(
            () => _propertyDomainService.ValidatePropertyRegistrationAsync(code, units));
        
        exception.Message.Should().Contain("at least one unit");
    }

    [Fact]
    public async Task ValidatePropertyRegistrationAsync_ShouldThrowException_WhenUnitsHaveDuplicates()
    {
        // Arrange
        var code = "TEST001";
        var units = new List<string> { "101", "102", "101" }; // Duplicate unit
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PropertyDomainException>(
            () => _propertyDomainService.ValidatePropertyRegistrationAsync(code, units));
        
        exception.Message.Should().Contain("duplicate unit numbers");
    }

    [Fact]
    public async Task ValidatePropertyRegistrationAsync_ShouldNotThrow_WhenDataIsValid()
    {
        // Arrange
        var code = "TEST001";
        var units = new List<string> { "101", "102", "103" };
        _propertyRepositoryMock.Setup(x => x.ExistsAsync(code, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);

        // Act & Assert
        await _propertyDomainService.ValidatePropertyRegistrationAsync(code, units);
        // Should not throw
    }
}