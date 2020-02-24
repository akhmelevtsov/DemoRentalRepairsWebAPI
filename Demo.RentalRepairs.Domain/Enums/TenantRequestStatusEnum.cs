namespace Demo.RentalRepairs.Domain.Enums
{
    public enum TenantRequestStatusEnum 
    {
        Undefined,
        RequestReceived,
        RequestRejected,
        WorkScheduled,
        WorkCompleted,
        WorkIncomplete,
        Closed
    }
}
