using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.Entities.Validators
{
    public class TenantValidator : AbstractValidator<Tenant>
    {
        public TenantValidator()
        {
            RuleFor(t => t.Property).NotNull();
            RuleFor(t => t.UnitNumber).SetValidator(new UnitNumberValidator());
            RuleFor(t => t.ContactInfo).SetValidator(new PersonContactInfoValidator());
            RuleFor(t => t).Must(t => t.Property.Units.Contains(t.UnitNumber)).WithMessage(t =>  $"There is no unit [{t.UnitNumber}] in property [{t.Property.Code}]");

        }
    }
}
 