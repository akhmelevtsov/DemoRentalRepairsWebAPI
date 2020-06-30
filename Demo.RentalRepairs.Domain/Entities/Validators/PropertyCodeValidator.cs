using FluentValidation;

namespace Demo.RentalRepairs.Domain.Entities.Validators
{
    public class PropertyCodeValidator : AbstractValidator<string>
    {
        public PropertyCodeValidator()
        {
            RuleFor(p => p).NotNull().MinimumLength(5).MaximumLength(10).WithName("Code");
        }
    }
}
