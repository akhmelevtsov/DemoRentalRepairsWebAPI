using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Commands.TenantRequests;

public class CreateTenantRequestCommand : ICommand<int>
{
    public int TenantId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string UrgencyLevel { get; set; } = default!;
}

public class SubmitTenantRequestCommand : ICommand
{
    public int TenantRequestId { get; set; }
}

public class ScheduleServiceWorkCommand : ICommand
{
    public int TenantRequestId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string WorkerEmail { get; set; } = default!;
    public string WorkOrderNumber { get; set; } = default!;
}

public class ReportWorkCompletedCommand : ICommand
{
    public int TenantRequestId { get; set; }
    public bool CompletedSuccessfully { get; set; }
    public string CompletionNotes { get; set; } = default!;
}

public class CloseRequestCommand : ICommand
{
    public int TenantRequestId { get; set; }
    public string ClosureNotes { get; set; } = default!;
}