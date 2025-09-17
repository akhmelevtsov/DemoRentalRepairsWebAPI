using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Events;

public class WorkerRegisteredEvent : BaseEvent
{
    public WorkerRegisteredEvent(Worker worker)
    {
        Worker = worker;
    }

    public Worker Worker { get; }
}

public class WorkerContactInfoChangedEvent : BaseEvent
{
    public WorkerContactInfoChangedEvent(Worker worker, PersonContactInfo oldContactInfo, PersonContactInfo newContactInfo)
    {
        Worker = worker;
        OldContactInfo = oldContactInfo;
        NewContactInfo = newContactInfo;
    }

    public Worker Worker { get; }
    public PersonContactInfo OldContactInfo { get; }
    public PersonContactInfo NewContactInfo { get; }
}

public class WorkerDeactivatedEvent : BaseEvent
{
    public WorkerDeactivatedEvent(Worker worker, string reason)
    {
        Worker = worker;
        Reason = reason;
    }

    public Worker Worker { get; }
    public string Reason { get; }
}

public class WorkerActivatedEvent : BaseEvent
{
    public WorkerActivatedEvent(Worker worker)
    {
        Worker = worker;
    }

    public Worker Worker { get; }
}