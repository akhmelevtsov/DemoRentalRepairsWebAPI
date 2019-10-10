using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class PersonContactInfoValidator : AbstractValidator< PersonContactInfo >
    {
        public PersonContactInfoValidator()
        {
            RuleFor(p => p.FirstName).NotNull();
            RuleFor(p => p.LastName).NotNull();
            RuleFor(p => p.EmailAddress).NotNull().EmailAddress();
            RuleFor(p => p.MobilePhone).NotNull();
        }
    }
}
