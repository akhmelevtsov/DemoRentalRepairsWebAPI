namespace Demo.RentalRepairs.Domain.Entities
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
