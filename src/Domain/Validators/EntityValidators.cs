using FluentValidation;
using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Domain.Validators;

public class PropertyValidator : AbstractValidator<Property>
{
    public PropertyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Property name is required")
            .Length(1, 100)
            .WithMessage("Property name must be between 1 and 100 characters");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Property code is required")
            .Length(1, 20)
            .WithMessage("Property code must be between 1 and 20 characters")
            .Matches(@"^[A-Za-z0-9\-_]+$")
            .WithMessage("Property code can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Phone number must be a valid format");

        RuleFor(x => x.NoReplyEmailAddress)
            .NotEmpty()
            .WithMessage("No-reply email address is required")
            .EmailAddress()
            .WithMessage("No-reply email address must be valid");

        RuleFor(x => x.Units)
            .NotEmpty()
            .WithMessage("Property must have at least one unit")
            .Must(units => units.Distinct().Count() == units.Count)
            .WithMessage("Property cannot have duplicate unit numbers");

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Property address is required")
            .SetValidator(new PropertyAddressValidator());

        RuleFor(x => x.Superintendent)
            .NotNull()
            .WithMessage("Superintendent information is required")
            .SetValidator(new PersonContactInfoValidator());
    }
}

public class TenantValidator : AbstractValidator<Tenant>
{
    public TenantValidator()
    {
        RuleFor(x => x.UnitNumber)
            .NotEmpty()
            .WithMessage("Unit number is required")
            .Length(1, 10)
            .WithMessage("Unit number must be between 1 and 10 characters");

        RuleFor(x => x.PropertyCode)
            .NotEmpty()
            .WithMessage("Property code is required");

        RuleFor(x => x.ContactInfo)
            .NotNull()
            .WithMessage("Contact information is required")
            .SetValidator(new PersonContactInfoValidator());
    }
}

public class TenantRequestValidator : AbstractValidator<TenantRequest>
{
    public TenantRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Request code is required")
            .Length(1, 50)
            .WithMessage("Request code must be between 1 and 50 characters");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Request title is required")
            .Length(1, 200)
            .WithMessage("Request title must be between 1 and 200 characters");

        RuleFor(x => x.Description)
            .Length(0, 1000)
            .WithMessage("Request description cannot exceed 1000 characters");

        RuleFor(x => x.UrgencyLevel)
            .NotEmpty()
            .WithMessage("Urgency level is required")
            .Must(BeValidUrgencyLevel)
            .WithMessage("Urgency level must be Low, Normal, High, or Critical");
    }

    private static bool BeValidUrgencyLevel(string urgencyLevel)
    {
        var validLevels = new[] { "Low", "Normal", "High", "Critical" };
        return validLevels.Contains(urgencyLevel);
    }
}

public class WorkerValidator : AbstractValidator<Worker>
{
    public WorkerValidator()
    {
        RuleFor(x => x.ContactInfo)
            .NotNull()
            .WithMessage("Contact information is required")
            .SetValidator(new PersonContactInfoValidator());

        RuleFor(x => x.Specialization)
            .Length(0, 100)
            .WithMessage("Specialization cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .Length(0, 500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}