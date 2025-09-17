using FluentValidation;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Validators.Properties;

public class RegisterPropertyCommandValidator : AbstractValidator<RegisterPropertyCommand>
{
    public RegisterPropertyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Property name is required")
            .MaximumLength(200).WithMessage("Property name must not exceed 200 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Property code is required")
            .MaximumLength(50).WithMessage("Property code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Property code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Phone number format is invalid");

        RuleFor(x => x.NoReplyEmailAddress) // Changed from NotificationEmail
            .NotEmpty().WithMessage("No-reply email address is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Units)
            .NotEmpty().WithMessage("At least one unit is required")
            .Must(units => units.Distinct().Count() == units.Count)
            .WithMessage("Duplicate unit numbers are not allowed");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required")
            .SetValidator(new PropertyAddressValidator());

        RuleFor(x => x.Superintendent)
            .NotNull().WithMessage("Superintendent information is required")
            .SetValidator(new PersonContactInfoValidator());
    }
}

public class PropertyAddressValidator : AbstractValidator<PropertyAddressDto>
{
    public PropertyAddressValidator()
    {
        RuleFor(x => x.StreetNumber)
            .NotEmpty().WithMessage("Street number is required")
            .MaximumLength(10).WithMessage("Street number must not exceed 10 characters");

        RuleFor(x => x.StreetName)
            .NotEmpty().WithMessage("Street name is required")
            .MaximumLength(100).WithMessage("Street name must not exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(50).WithMessage("City must not exceed 50 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(10).WithMessage("Postal code must not exceed 10 characters");
    }
}

public class PersonContactInfoValidator : AbstractValidator<PersonContactInfoDto>
{
    public PersonContactInfoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.MobilePhone) // Changed from PhoneNumber
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.MobilePhone));
    }
}