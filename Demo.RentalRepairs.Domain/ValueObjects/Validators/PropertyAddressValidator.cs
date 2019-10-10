using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class PropertyAddressValidator : AbstractValidator< PropertyAddress>
    {
        public PropertyAddressValidator()
        {
            RuleFor(a => a.StreetNumber).NotNull();
            RuleFor(a => a.StreetName).NotNull();
            RuleFor(a => a.City).NotNull();
            RuleFor(a => a.PostalCode).NotNull();
        }
    }
}
