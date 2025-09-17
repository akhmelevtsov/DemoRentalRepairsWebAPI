using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Commands.Properties.Handlers;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.ValueObjects;
using Xunit;

namespace RentalRepairs.Application.Tests.Commands.Properties;

public class RegisterPropertyCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_Methods_And_Return_Property_Id()
    {
        // Arrange
        var propertyRepositoryMock = new Mock<IPropertyRepository>();
        
        // Create real domain service for testing
        var propertyDomainService = new PropertyDomainService(
            propertyRepositoryMock.Object,
            Mock.Of<ITenantRepository>(),
            Mock.Of<DomainValidationService>());
        
        var handler = new RegisterPropertyCommandHandler(
            propertyRepositoryMock.Object,
            propertyDomainService);

        var command = new RegisterPropertyCommand
        {
            Name = "Test Property",
            Code = "TP-001",
            PhoneNumber = "555-1234",
            NoReplyEmailAddress = "noreply@test.com",
            Units = new List<string> { "101", "102", "103" },
            Address = new PropertyAddressDto
            {
                StreetNumber = "123",
                StreetName = "Main St",
                City = "Test City",
                PostalCode = "12345"
            },
            Superintendent = new PersonContactInfoDto
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john@test.com"
            }
        };

        // Mock a property with a specific ID to be returned
        Property? capturedProperty = null;

        // Set up repository mocks for domain service validation
        propertyRepositoryMock
            .Setup(x => x.ExistsAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        propertyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .Callback<Property, CancellationToken>((property, _) => 
            {
                capturedProperty = property;
                // Simulate database setting the ID
                property.Id = 123;
            })
            .Returns(Task.CompletedTask);

        propertyRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(123); // Should return the ID we set
        propertyRepositoryMock.Verify(x => x.ExistsAsync(command.Code, It.IsAny<CancellationToken>()), Times.Once);
        propertyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
        propertyRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        // Verify the property was created with correct data
        capturedProperty.Should().NotBeNull();
        capturedProperty!.Name.Should().Be(command.Name);
        capturedProperty.Code.Should().Be(command.Code);
        capturedProperty.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Handle_Should_Create_Property_With_Correct_Value_Objects()
    {
        // Arrange
        var propertyRepositoryMock = new Mock<IPropertyRepository>();
        
        var propertyDomainService = new PropertyDomainService(
            propertyRepositoryMock.Object,
            Mock.Of<ITenantRepository>(),
            Mock.Of<DomainValidationService>());
        
        var handler = new RegisterPropertyCommandHandler(
            propertyRepositoryMock.Object,
            propertyDomainService);

        var command = new RegisterPropertyCommand
        {
            Name = "Test Property",
            Code = "TP-001",
            PhoneNumber = "555-1234",
            NoReplyEmailAddress = "noreply@test.com",
            Units = new List<string> { "101", "102" },
            Address = new PropertyAddressDto
            {
                StreetNumber = "123",
                StreetName = "Main St",
                City = "Test City",
                PostalCode = "12345"
            },
            Superintendent = new PersonContactInfoDto
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john@test.com"
            }
        };

        Property? capturedProperty = null;

        propertyRepositoryMock
            .Setup(x => x.ExistsAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        propertyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .Callback<Property, CancellationToken>((property, _) => 
            {
                capturedProperty = property;
                property.Id = 456; // Set a different ID
            })
            .Returns(Task.CompletedTask);

        propertyRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(456);
        capturedProperty.Should().NotBeNull();
        
        // Verify Address value object
        capturedProperty!.Address.Should().NotBeNull();
        capturedProperty.Address.StreetNumber.Should().Be("123");
        capturedProperty.Address.StreetName.Should().Be("Main St");
        capturedProperty.Address.City.Should().Be("Test City");
        capturedProperty.Address.PostalCode.Should().Be("12345");
        
        // Verify Superintendent value object
        capturedProperty.Superintendent.Should().NotBeNull();
        capturedProperty.Superintendent.FirstName.Should().Be("John");
        capturedProperty.Superintendent.LastName.Should().Be("Doe");
        capturedProperty.Superintendent.EmailAddress.Should().Be("john@test.com");
        
        // Verify Units
        capturedProperty.Units.Should().HaveCount(2);
        capturedProperty.Units.Should().Contain("101");
        capturedProperty.Units.Should().Contain("102");
    }
}