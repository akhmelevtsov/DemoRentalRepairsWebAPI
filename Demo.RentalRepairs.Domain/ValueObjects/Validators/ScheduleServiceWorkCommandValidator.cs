using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using FluentValidation;

namespace Demo.RentalRepairs.Domain.ValueObjects.Validators
{
    public class ScheduleServiceWorkCommandValidator : AbstractValidator<ScheduleServiceWorkCommand>
    {
        public ScheduleServiceWorkCommandValidator()
        {
            RuleFor(t => t.WorkerEmailAddress).EmailAddress();
            RuleFor(t  => t.ServiceDate).GreaterThanOrEqualTo( PropertyDomainService.DateTimeProvider.GetDateTime().Date.AddDays(1));
            RuleFor(t => t.WorkOrderNo).GreaterThan(0);

        }
    }
}
