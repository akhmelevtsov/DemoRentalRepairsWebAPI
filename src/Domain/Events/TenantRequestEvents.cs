using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Events;

public class TenantRequestSubmittedEvent : BaseEvent
{
    public TenantRequestSubmittedEvent(TenantRequest tenantRequest)
    {
        TenantRequest = tenantRequest;
    }

    public TenantRequest TenantRequest { get; }
}

public class TenantRequestDeclinedEvent : BaseEvent
{
    public TenantRequestDeclinedEvent(TenantRequest tenantRequest, string reason)
    {
        TenantRequest = tenantRequest;
        Reason = reason;
    }

    public TenantRequest TenantRequest { get; }
    public string Reason { get; }
}

public class TenantRequestScheduledEvent : BaseEvent
{
    public TenantRequestScheduledEvent(TenantRequest tenantRequest, ServiceWorkScheduleInfo scheduleInfo)
    {
        TenantRequest = tenantRequest;
        ScheduleInfo = scheduleInfo;
    }

    public TenantRequest TenantRequest { get; }
    public ServiceWorkScheduleInfo ScheduleInfo { get; }
}

public class TenantRequestCompletedEvent : BaseEvent
{
    public TenantRequestCompletedEvent(TenantRequest tenantRequest, string notes)
    {
        TenantRequest = tenantRequest;
        Notes = notes;
    }

    public TenantRequest TenantRequest { get; }
    public string Notes { get; }
}

public class TenantRequestFailedEvent : BaseEvent
{
    public TenantRequestFailedEvent(TenantRequest tenantRequest, string notes)
    {
        TenantRequest = tenantRequest;
        Notes = notes;
    }

    public TenantRequest TenantRequest { get; }
    public string Notes { get; }
}

public class TenantRequestClosedEvent : BaseEvent
{
    public TenantRequestClosedEvent(TenantRequest tenantRequest, string closureNotes)
    {
        TenantRequest = tenantRequest;
        ClosureNotes = closureNotes;
    }

    public TenantRequest TenantRequest { get; }
    public string ClosureNotes { get; }
}

public class TenantContactInfoChangedEvent : BaseEvent
{
    public TenantContactInfoChangedEvent(Tenant tenant, PersonContactInfo oldContactInfo, PersonContactInfo newContactInfo)
    {
        Tenant = tenant;
        OldContactInfo = oldContactInfo;
        NewContactInfo = newContactInfo;
    }

    public Tenant Tenant { get; }
    public PersonContactInfo OldContactInfo { get; }
    public PersonContactInfo NewContactInfo { get; }
}