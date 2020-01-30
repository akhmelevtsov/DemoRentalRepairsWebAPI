using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class PersonContactInfoValidator : AbstractValidator< PersonContactInfo >
    {
        public PersonContactInfoValidator()
        {
            RuleFor(p => p.FirstName).NotNull().Matches(@"^[a-zA-Z]+(([\'\,\.\-][a-zA-Z])?[a-zA-Z]*)*$");
            RuleFor(p => p.LastName).NotNull().Matches(@"^[a-zA-Z]+(([\'\,\.\-][a-zA-Z])?[a-zA-Z]*)*$");
            RuleFor(p => p.EmailAddress).NotNull().EmailAddress();
            RuleFor(p => p.MobilePhone).NotNull().Matches(@"^[2-9]\d{2}-\d{3}-\d{4}$");
        }
    }
}
