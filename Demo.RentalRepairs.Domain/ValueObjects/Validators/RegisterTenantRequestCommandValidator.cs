using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class RegisterTenantRequestCommandValidator : AbstractValidator<RegisterTenantRequestCommand>
    {
        public RegisterTenantRequestCommandValidator()
        {
            RuleFor(p => p.Title ).NotNull().MinimumLength(10).MaximumLength(50);
            RuleFor(p => p.Description ).NotNull().MinimumLength(10).MaximumLength(5000);
        }
    }
}
