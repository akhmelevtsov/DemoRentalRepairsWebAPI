using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Entities.Validators;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class AddPropertyCommandValidator : AbstractValidator<AddPropertyCommand>
    {
        public AddPropertyCommandValidator()
        {
            RuleFor(p => p.Code).NotNull().SetValidator(new PropertyCodeValidator());
            RuleFor(p => p.Name).NotNull().MinimumLength(5).MaximumLength(50);
            RuleFor(p => p.PhoneNumber).NotNull().Matches(@"^[2-9]\d{2}-\d{3}-\d{4}$");
            RuleFor(p => p.Address).SetValidator(new PropertyAddressValidator());
            RuleFor(p => p.Superintendent).SetValidator(new PersonContactInfoValidator());
            RuleFor(p => p.NoReplyEmailAddress).NotNull().EmailAddress();
            RuleFor(p => p.Units).NotNull()
                .Must(p => p.Count >= 2).WithMessage("No less than 2 units are allowed")
                .ForEach(u => u.SetValidator(new UnitNumberValidator())).Must(p => !HasDuplicates(p)).WithMessage("All unit numbers should be unique");

        }

        private bool HasDuplicates(IEnumerable<string> lst)
        {
            var duplicates = lst.GroupBy(s => s)
                .SelectMany(grp => grp.Skip(1));
            return duplicates.Any();
        }
    }
}
