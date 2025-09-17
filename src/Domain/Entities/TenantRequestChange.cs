using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Entities;

public class TenantRequestChange : BaseEntity
{
    protected TenantRequestChange() { } // For EF Core

    public TenantRequestChange(
        TenantRequestStatus status,
        string description,
        int workOrderNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Status = status;
        Description = description;
        WorkOrderNumber = workOrderNumber;
    }

    public TenantRequestStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int WorkOrderNumber { get; private set; }
}