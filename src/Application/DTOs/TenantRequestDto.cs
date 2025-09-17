using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Application.DTOs;

public class TenantRequestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TenantRequestStatus Status { get; set; }
    public string UrgencyLevel { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    
    // Related entity information
    public int TenantId { get; set; }
    public TenantDto Tenant { get; set; } = default!;
    
    // Work scheduling information
    public string? AssignedWorkerEmail { get; set; }
    public string? WorkOrderNumber { get; set; }
    public bool? WorkCompletedSuccessfully { get; set; }
    public string? CompletionNotes { get; set; }
    public string? ClosureNotes { get; set; }
    
    // Change history
    public List<TenantRequestChangeDto> RequestChanges { get; set; } = new();
}

public class TenantRequestChangeDto
{
    public int Id { get; set; }
    public TenantRequestStatus Status { get; set; }
    public string Description { get; set; } = default!;
    public DateTime ChangeDate { get; set; }
    public string? ChangedByEmail { get; set; }
}