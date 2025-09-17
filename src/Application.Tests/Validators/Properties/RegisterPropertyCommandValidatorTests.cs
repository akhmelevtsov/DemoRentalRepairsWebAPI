using FluentValidation.TestHelper;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Validators.Properties;
using Xunit;

namespace RentalRepairs.Application.Tests.Validators.Properties;

public class RegisterPropertyCommandValidatorTests
{
    private readonly RegisterPropertyCommandValidator _validator;

    public RegisterPropertyCommandValidatorTests()
    {
        _validator = new RegisterPropertyCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new RegisterPropertyCommand { Name = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Code_Is_Empty()
    {
        var command = new RegisterPropertyCommand { Code = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_Have_Error_When_Code_Has_Invalid_Format()
    {
        var command = new RegisterPropertyCommand { Code = "invalid code!" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_Have_Error_When_Units_Are_Empty()
    {
        var command = new RegisterPropertyCommand { Units = new List<string>() };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Units);
    }

    [Fact]
    public void Should_Have_Error_When_Units_Have_Duplicates()
    {
        var command = new RegisterPropertyCommand 
        { 
            Units = new List<string> { "101", "102", "101" } 
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Units);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new RegisterPropertyCommand
        {
            Name = "Test Property",
            Code = "TP-001",
            PhoneNumber = "555-1234",
            NoReplyEmailAddress = "test@example.com", // Changed from NotificationEmail
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
                EmailAddress = "john@example.com"
            }
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}