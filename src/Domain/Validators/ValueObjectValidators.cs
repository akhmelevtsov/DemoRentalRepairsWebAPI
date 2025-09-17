using FluentValidation;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Validators;

public class PersonContactInfoValidator : AbstractValidator<PersonContactInfo>
{
    public PersonContactInfoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .Length(1, 50)
            .WithMessage("First name must be between 1 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .Length(1, 50)
            .WithMessage("Last name must be between 1 and 50 characters");

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress()
            .WithMessage("Email address must be valid");

        RuleFor(x => x.MobilePhone)
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.MobilePhone))
            .WithMessage("Mobile phone must be a valid phone number");
    }
}

public class PropertyAddressValidator : AbstractValidator<PropertyAddress>
{
    public PropertyAddressValidator()
    {
        RuleFor(x => x.StreetNumber)
            .NotEmpty()
            .WithMessage("Street number is required")
            .Length(1, 10)
            .WithMessage("Street number must be between 1 and 10 characters");

        RuleFor(x => x.StreetName)
            .NotEmpty()
            .WithMessage("Street name is required")
            .Length(1, 100)
            .WithMessage("Street name must be between 1 and 100 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .Length(1, 50)
            .WithMessage("City must be between 1 and 50 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage("Postal code is required")
            .Matches(@"^[A-Za-z0-9\s\-]{3,10}$")
            .WithMessage("Postal code must be a valid format");
    }
}