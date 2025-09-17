namespace RentalRepairs.Domain.Enums;

public enum TenantRequestStatus
{
    Draft = 0,
    Submitted = 1,
    Declined = 2,
    Scheduled = 3,
    Done = 4,
    Failed = 5,
    Closed = 6
}

public static class TenantRequestStatusExtensions
{
    public static string GetDisplayName(this TenantRequestStatus status)
    {
        return status switch
        {
            TenantRequestStatus.Draft => "Draft",
            TenantRequestStatus.Submitted => "Submitted",
            TenantRequestStatus.Declined => "Declined",
            TenantRequestStatus.Scheduled => "Scheduled",
            TenantRequestStatus.Done => "Completed",
            TenantRequestStatus.Failed => "Failed",
            TenantRequestStatus.Closed => "Closed",
            _ => status.ToString()
        };
    }

    public static bool IsTerminal(this TenantRequestStatus status)
    {
        return status is TenantRequestStatus.Closed or TenantRequestStatus.Declined;
    }

    public static bool CanBeModified(this TenantRequestStatus status)
    {
        return status is TenantRequestStatus.Draft or TenantRequestStatus.Submitted;
    }
}