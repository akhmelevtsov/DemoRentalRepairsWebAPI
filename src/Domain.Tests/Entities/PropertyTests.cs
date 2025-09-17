using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace RentalRepairs.Domain.Tests.Entities;

public class PropertyTests
{
    [Fact]
    public void Property_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var name = "Sunset Apartments";
        var code = "SA001";
        var address = new PropertyAddress("123", "Main Street", "Anytown", "12345");
        var phoneNumber = "+1-555-1234";
        var superintendent = new PersonContactInfo("John", "Doe", "john.doe@example.com", "+1-555-5678");
        var units = new List<string> { "101", "102", "103" };
        var noReplyEmail = "noreply@sunsetapartments.com";

        // Act
        var property = new Property(name, code, address, phoneNumber, superintendent, units, noReplyEmail);

        // Assert
        property.Name.Should().Be(name);
        property.Code.Should().Be(code);
        property.Address.Should().Be(address);
        property.PhoneNumber.Should().Be(phoneNumber);
        property.Superintendent.Should().Be(superintendent);
        property.Units.Should().BeEquivalentTo(units);
        property.NoReplyEmailAddress.Should().Be(noReplyEmail);
        property.DomainEvents.Should().HaveCount(1);
        property.DomainEvents.First().Should().BeOfType<Domain.Events.PropertyRegisteredEvent>();
    }

    [Fact]
    public void RegisterTenant_ShouldCreateTenant_WhenUnitIsAvailable()
    {
        // Arrange
        var property = CreateTestProperty();
        var contactInfo = new PersonContactInfo("Jane", "Smith", "jane.smith@example.com");

        // Act
        var tenant = property.RegisterTenant(contactInfo, "101");

        // Assert
        tenant.Should().NotBeNull();
        tenant.ContactInfo.Should().Be(contactInfo);
        tenant.UnitNumber.Should().Be("101");
        tenant.Property.Should().Be(property);
        property.Tenants.Should().Contain(tenant);
        property.DomainEvents.Should().HaveCount(2); // PropertyRegistered + TenantRegistered
    }

    [Fact]
    public void RegisterTenant_ShouldThrowException_WhenUnitDoesNotExist()
    {
        // Arrange
        var property = CreateTestProperty();
        var contactInfo = new PersonContactInfo("Jane", "Smith", "jane.smith@example.com");

        // Act & Assert
        Action act = () => property.RegisterTenant(contactInfo, "999");
        act.Should().Throw<PropertyDomainException>()
           .WithMessage("Unit 999 does not exist in property SA001");
    }

    private static Property CreateTestProperty()
    {
        return new Property(
            "Sunset Apartments",
            "SA001",
            new PropertyAddress("123", "Main Street", "Anytown", "12345"),
            "+1-555-1234",
            new PersonContactInfo("John", "Doe", "john.doe@example.com"),
            new List<string> { "101", "102", "103" },
            "noreply@sunsetapartments.com");
    }
}