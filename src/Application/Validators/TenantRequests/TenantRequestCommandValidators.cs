using FluentValidation;
using RentalRepairs.Application.Commands.TenantRequests;

namespace RentalRepairs.Application.Validators.TenantRequests;

public class CreateTenantRequestCommandValidator : AbstractValidator<CreateTenantRequestCommand>
{
    public CreateTenantRequestCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .GreaterThan(0).WithMessage("Valid tenant ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Request title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Request description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.UrgencyLevel)
            .NotEmpty().WithMessage("Urgency level is required")
            .Must(BeValidUrgencyLevel).WithMessage("Invalid urgency level. Valid values are: Low, Normal, High, Critical");
    }

    private static bool BeValidUrgencyLevel(string urgencyLevel)
    {
        var validLevels = new[] { "Low", "Normal", "High", "Critical" };
        return validLevels.Contains(urgencyLevel);
    }
}

public class ScheduleServiceWorkCommandValidator : AbstractValidator<ScheduleServiceWorkCommand>
{
    public ScheduleServiceWorkCommandValidator()
    {
        RuleFor(x => x.TenantRequestId)
            .GreaterThan(0).WithMessage("Valid tenant request ID is required");

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled date must be in the future");

        RuleFor(x => x.WorkerEmail)
            .NotEmpty().WithMessage("Worker email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.WorkOrderNumber)
            .NotEmpty().WithMessage("Work order number is required")
            .MaximumLength(50).WithMessage("Work order number must not exceed 50 characters");
    }
}

public class ReportWorkCompletedCommandValidator : AbstractValidator<ReportWorkCompletedCommand>
{
    public ReportWorkCompletedCommandValidator()
    {
        RuleFor(x => x.TenantRequestId)
            .GreaterThan(0).WithMessage("Valid tenant request ID is required");

        RuleFor(x => x.CompletionNotes)
            .NotEmpty().WithMessage("Completion notes are required")
            .MaximumLength(1000).WithMessage("Completion notes must not exceed 1000 characters");
    }
}