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
            RuleFor(p => p).NotNull().MinimumLength(2).MaximumLength(6);
        }
    }
}
