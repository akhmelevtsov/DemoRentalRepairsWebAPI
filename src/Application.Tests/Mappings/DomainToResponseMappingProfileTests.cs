using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Mappings;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;
using Xunit;
using FluentAssertions;
using Mapster;

namespace RentalRepairs.Application.Tests.Mappings;

public class DomainToResponseMappingProfileTests
{
    public DomainToResponseMappingProfileTests()
    {
        // Register Mapster mappings
        DomainToResponseMappingConfig.RegisterMappings();
    }

    [Fact]
    public void Should_Map_Property_To_PropertyDto()
    {
        // Arrange
        var address = new PropertyAddress("123", "Main St", "Test City", "12345");
        var superintendent = new PersonContactInfo("John", "Doe", "john@example.com");
        var property = new Property(
            "Test Property",
            "TP-001",
            address,
            "555-1234",
            superintendent,
            new List<string> { "101", "102" },
            "notify@example.com");

        // Act
        var dto = property.Adapt<PropertyDto>();

        // Assert
        dto.Should().NotBeNull();
        dto.Name.Should().Be("Test Property");
        dto.Code.Should().Be("TP-001");
        dto.PhoneNumber.Should().Be("555-1234");
        dto.NoReplyEmailAddress.Should().Be("notify@example.com");
        dto.Units.Should().HaveCount(2);
        dto.Address.Should().NotBeNull();
        dto.Address.FullAddress.Should().Be("123 Main St, Test City, 12345");
        dto.Superintendent.Should().NotBeNull();
        dto.Superintendent.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void Should_Map_PersonContactInfo_To_PersonContactInfoDto()
    {
        // Arrange
        var contactInfo = new PersonContactInfo("Jane", "Smith", "jane@example.com", "555-5678");

        // Act
        var dto = contactInfo.Adapt<PersonContactInfoDto>();

        // Assert
        dto.Should().NotBeNull();
        dto.FirstName.Should().Be("Jane");
        dto.LastName.Should().Be("Smith");
        dto.EmailAddress.Should().Be("jane@example.com");
        dto.MobilePhone.Should().Be("555-5678");
        dto.FullName.Should().Be("Jane Smith");
    }

    [Fact]
    public void Should_Map_PropertyAddress_To_PropertyAddressDto()
    {
        // Arrange
        var address = new PropertyAddress("456", "Oak Ave", "Another City", "67890");

        // Act
        var dto = address.Adapt<PropertyAddressDto>();

        // Assert
        dto.Should().NotBeNull();
        dto.StreetNumber.Should().Be("456");
        dto.StreetName.Should().Be("Oak Ave");
        dto.City.Should().Be("Another City");
        dto.PostalCode.Should().Be("67890");
        dto.FullAddress.Should().Be("456 Oak Ave, Another City, 67890");
    }

    [Fact]
    public void Should_Map_Worker_To_WorkerDto()
    {
        // Arrange
        var contactInfo = new PersonContactInfo("Bob", "Builder", "bob@workers.com");
        var worker = new Worker(contactInfo);
        worker.SetSpecialization("Plumbing");

        // Act
        var dto = worker.Adapt<WorkerDto>();

        // Assert
        dto.Should().NotBeNull();
        dto.ContactInfo.FullName.Should().Be("Bob Builder");
        dto.ContactInfo.EmailAddress.Should().Be("bob@workers.com");
        dto.Specialization.Should().Be("Plumbing");
        dto.IsActive.Should().BeTrue();
    }
}