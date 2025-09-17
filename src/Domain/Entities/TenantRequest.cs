using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Enums;
using RentalRepairs.Domain.Events;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Entities;

public class TenantRequest : BaseEntity, IAggregateRoot
{
    private readonly List<TenantRequestChange> _requestChanges = new();
    private readonly List<(TenantRequestStatus next, TenantRequestStatus prev)> _allowedTransitions = new()
    {
        (TenantRequestStatus.Submitted, TenantRequestStatus.Draft),
        (TenantRequestStatus.Declined, TenantRequestStatus.Submitted),
        (TenantRequestStatus.Scheduled, TenantRequestStatus.Submitted),
        (TenantRequestStatus.Failed, TenantRequestStatus.Scheduled),
        (TenantRequestStatus.Done, TenantRequestStatus.Scheduled),
        (TenantRequestStatus.Scheduled, TenantRequestStatus.Failed), // Reschedule
        (TenantRequestStatus.Closed, TenantRequestStatus.Done),
        (TenantRequestStatus.Closed, TenantRequestStatus.Declined)
    };

    protected TenantRequest() { } // For EF Core

    public TenantRequest(
        Tenant tenant,
        string code,
        string title,
        string description,
        string urgencyLevel)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Request code cannot be empty", nameof(code));
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Request title cannot be empty", nameof(title));

        Tenant = tenant;
        TenantId = tenant.Id; // Set the foreign key
        Code = code;
        Title = title;
        Description = description ?? string.Empty;
        UrgencyLevel = urgencyLevel ?? "Normal";
        Status = TenantRequestStatus.Draft;
        ServiceWorkOrderCount = 0;

        AddRequestChange(Status, $"Request created: {title}");
    }

    // Foreign key property for EF Core
    public int TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string UrgencyLevel { get; private set; } = string.Empty;
    public TenantRequestStatus Status { get; private set; }
    public int ServiceWorkOrderCount { get; private set; }

    public IReadOnlyCollection<TenantRequestChange> RequestChanges => _requestChanges.AsReadOnly();

    // Computed properties for easy access
    public string TenantFullName => Tenant?.ContactInfo?.GetFullName() ?? string.Empty;
    public string PropertyName => Tenant?.Property?.Name ?? string.Empty;
    public string SuperintendentFullName => Tenant?.Property?.Superintendent?.GetFullName() ?? string.Empty;
    public string PropertyId => Tenant?.Property?.Id.ToString() ?? string.Empty;
    public string TenantIdentifier => Tenant?.Id.ToString() ?? string.Empty; // Renamed from TenantId to avoid conflict
    public string TenantUnit => Tenant?.UnitNumber ?? string.Empty;
    public string PropertyNoReplyEmail => Tenant?.Property?.NoReplyEmailAddress ?? string.Empty;
    public string TenantEmail => Tenant?.ContactInfo?.EmailAddress ?? string.Empty;
    public string PropertyPhone => Tenant?.Property?.PhoneNumber ?? string.Empty;
    public string SuperintendentEmail => Tenant?.Property?.Superintendent?.EmailAddress ?? string.Empty;

    public void Submit()
    {
        ChangeStatus(TenantRequestStatus.Submitted, "Request submitted for review");
        AddDomainEvent(new TenantRequestSubmittedEvent(this));
    }

    public void Decline(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Decline reason cannot be empty", nameof(reason));

        ChangeStatus(TenantRequestStatus.Declined, $"Request declined: {reason}");
        AddDomainEvent(new TenantRequestDeclinedEvent(this, reason));
    }

    public void Schedule(DateTime serviceDate, string workerEmail, string workOrderNumber)
    {
        if (serviceDate <= DateTime.UtcNow)
            throw new TenantRequestDomainException("Service date must be in the future");
        
        if (string.IsNullOrWhiteSpace(workerEmail))
            throw new ArgumentException("Worker email cannot be empty", nameof(workerEmail));
        
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new ArgumentException("Work order number cannot be empty", nameof(workOrderNumber));

        ServiceWorkOrderCount++;
        
        var scheduleInfo = new ServiceWorkScheduleInfo(
            serviceDate,
            workerEmail,
            workOrderNumber,
            ServiceWorkOrderCount);

        ChangeStatus(TenantRequestStatus.Scheduled, 
            $"Work scheduled for {serviceDate:yyyy-MM-dd} with worker {workerEmail}, Work Order: {workOrderNumber}");

        AddDomainEvent(new TenantRequestScheduledEvent(this, scheduleInfo));
    }

    public void ReportWorkCompleted(bool success, string notes)
    {
        var newStatus = success ? TenantRequestStatus.Done : TenantRequestStatus.Failed;
        var message = success 
            ? $"Work completed successfully. Notes: {notes}"
            : $"Work failed. Notes: {notes}";

        ChangeStatus(newStatus, message);

        if (success)
        {
            AddDomainEvent(new TenantRequestCompletedEvent(this, notes));
        }
        else
        {
            AddDomainEvent(new TenantRequestFailedEvent(this, notes));
        }
    }

    public void Close(string closureNotes = "")
    {
        ChangeStatus(TenantRequestStatus.Closed, $"Request closed. {closureNotes}".Trim());
        AddDomainEvent(new TenantRequestClosedEvent(this, closureNotes));
    }

    private void ChangeStatus(TenantRequestStatus newStatus, string changeDescription)
    {
        if (!_allowedTransitions.Any(t => t.next == newStatus && t.prev == Status))
        {
            throw new TenantRequestDomainException(
                $"Cannot change status from {Status} to {newStatus}");
        }

        Status = newStatus;
        AddRequestChange(newStatus, changeDescription);
    }

    private void AddRequestChange(TenantRequestStatus status, string description)
    {
        var change = new TenantRequestChange(
            status,
            description,
            ServiceWorkOrderCount);

        _requestChanges.Add(change);
    }
}