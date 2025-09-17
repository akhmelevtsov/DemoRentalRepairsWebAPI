using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.Properties;
using Xunit;
using FluentAssertions;

namespace RentalRepairs.Application.Tests.Commands;

public class CQRSInfrastructureTests
{
    [Fact]
    public void RegisterPropertyCommand_Should_Implement_ICommand()
    {
        // Arrange & Act
        var command = new RegisterPropertyCommand();

        // Assert
        command.Should().BeAssignableTo<ICommand<int>>();
    }

    [Fact]
    public void GetPropertyByIdQuery_Should_Implement_IQuery()
    {
        // Arrange & Act
        var query = new GetPropertyByIdQuery(1);

        // Assert
        query.Should().BeAssignableTo<IQuery<PropertyDto>>();
    }

    [Fact]
    public void RegisterPropertyCommand_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var command = new RegisterPropertyCommand
        {
            Name = "Test Property",
            Code = "TP-001",
            PhoneNumber = "555-1234",
            NoReplyEmailAddress = "test@example.com", // Changed from NotificationEmail
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
                EmailAddress = "john@example.com"
            }
        };

        // Assert
        command.Name.Should().Be("Test Property");
        command.Code.Should().Be("TP-001");
        command.PhoneNumber.Should().Be("555-1234");
        command.NoReplyEmailAddress.Should().Be("test@example.com"); // Changed from NotificationEmail
        command.Units.Should().HaveCount(2);
        command.Address.Should().NotBeNull();
        command.Superintendent.Should().NotBeNull();
    }

    [Fact]
    public void GetPropertyByIdQuery_Should_Have_PropertyId()
    {
        // Arrange & Act
        var query = new GetPropertyByIdQuery(123);

        // Assert
        query.PropertyId.Should().Be(123);
    }

    [Fact]
    public void PropertyDto_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var dto = new PropertyDto
        {
            Id = 1,
            Name = "Test Property",
            Code = "TP-001",
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
                EmailAddress = "john@example.com"
            },
            Units = new List<string> { "101", "102" },
            Tenants = new List<TenantDto>()
        };

        // Assert
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Test Property");
        dto.Code.Should().Be("TP-001");
        dto.Address.Should().NotBeNull();
        dto.Superintendent.Should().NotBeNull();
        dto.Units.Should().HaveCount(2);
        dto.Tenants.Should().NotBeNull();
    }
}