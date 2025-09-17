using RentalRepairs.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace RentalRepairs.Domain.Tests.ValueObjects;

public class PersonContactInfoTests
{
    [Fact]
    public void PersonContactInfo_ShouldBeCreated_WithValidData()
    {
        // Arrange & Act
        var contactInfo = new PersonContactInfo("John", "Doe", "john.doe@example.com", "+1-555-1234");

        // Assert
        contactInfo.FirstName.Should().Be("John");
        contactInfo.LastName.Should().Be("Doe");
        contactInfo.EmailAddress.Should().Be("john.doe@example.com");
        contactInfo.MobilePhone.Should().Be("+1-555-1234");
        contactInfo.GetFullName().Should().Be("John Doe");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PersonContactInfo_ShouldThrowException_WhenFirstNameIsInvalid(string firstName)
    {
        // Act & Assert
        Action act = () => new PersonContactInfo(firstName, "Doe", "john.doe@example.com");
        act.Should().Throw<ArgumentException>()
           .WithMessage("First name cannot be empty*");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("john.doe@")]
    [InlineData("john.doe")]
    public void PersonContactInfo_ShouldThrowException_WhenEmailIsInvalid(string email)
    {
        // Act & Assert
        Action act = () => new PersonContactInfo("John", "Doe", email);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Invalid email address format*");
    }

    [Fact]
    public void PersonContactInfo_ShouldNormalizeEmail_ToLowerCase()
    {
        // Arrange & Act
        var contactInfo = new PersonContactInfo("John", "Doe", "JOHN.DOE@EXAMPLE.COM");

        // Assert
        contactInfo.EmailAddress.Should().Be("john.doe@example.com");
    }
}