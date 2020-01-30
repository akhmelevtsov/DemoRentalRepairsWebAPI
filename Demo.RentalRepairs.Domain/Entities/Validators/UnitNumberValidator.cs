using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.Entities.Validators
{
    public class UnitNumberValidator : AbstractValidator<string>
    {
        public UnitNumberValidator()
        {
            RuleFor(p => p).NotNull().MinimumLength(2).WithMessage(p => $"The length of '{p}' should be at least 2 characters").MaximumLength(6).WithMessage(p => $"The length of '{p}' exceeds 6 characters" );
        }
    }
}
