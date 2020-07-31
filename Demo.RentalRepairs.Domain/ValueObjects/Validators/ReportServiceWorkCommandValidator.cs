using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class ReportServiceWorkCommandValidator : AbstractValidator<ReportServiceWorkCommand>
    {
        public ReportServiceWorkCommandValidator()
        {
            RuleFor(t => t.Notes).NotEmpty().When(t => t.Success == false)
                .WithMessage("Provide reason why work cannot be completed");


        }
    }
}
