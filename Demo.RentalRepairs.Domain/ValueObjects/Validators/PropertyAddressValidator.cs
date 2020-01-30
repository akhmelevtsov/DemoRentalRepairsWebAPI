using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class PropertyAddressValidator : AbstractValidator< PropertyAddress>
    {
        public PropertyAddressValidator()
        {
            RuleFor(a => a.StreetNumber).NotNull().NotEmpty().Matches(@"((\d{1,6}\-\d{1,6})|(\d{1,6}\\\d{1,6})|(\d{1,6})(\/)(\d{1,6})|(\w{1}\-?\d{1,6})|(\w{1}\s\d{1,6})|((P\.?O\.?\s)((BOX)|(Box))(\s\d{1,6}))|((([R]{2})|([H][C]))(\s\d{1,6}\s)((BOX)|(Box))(\s\d{1,6}))?)$");
            RuleFor(a => a.StreetName).NotNull().NotEmpty().Matches(@"^([a-zA-z\s]{2,})$");
            RuleFor(a => a.City).NotNull().NotEmpty().Matches(@"^([a-zA-z\s]{2,})$") ;
            RuleFor(a => a.PostalCode).NotNull().Matches(@"^((\d{5}-\d{4})|(\d{5})|([A-Z]\d[A-Z]\s\d[A-Z]\d))$");
        }
    }
}
