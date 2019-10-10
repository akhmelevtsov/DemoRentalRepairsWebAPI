using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.Entities.Validators
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        public PropertyValidator()
        {
            RuleFor(p => p.Code).SetValidator(new PropertyCodeValidator());
            RuleFor(p => p.Name).NotNull();
            RuleFor(p => p.PhoneNumber).NotNull();
            RuleFor(p => p.Name).NotNull();
            RuleFor(p => p.Address).SetValidator(new PropertyAddressValidator());
            RuleFor(p => p.Superintendent).SetValidator(new PersonContactInfoValidator());
            RuleFor(p => p.Units).NotNull()
                .Must(p => p.Count >= 2).WithMessage("No less than 2 units are allowed")
                .ForEach(u => u.SetValidator(new UnitNumberValidator()));
        }
    }
}
