using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Domain.Events;

public class PropertyRegisteredEvent : BaseEvent
{
    public PropertyRegisteredEvent(Property property)
    {
        Property = property;
    }

    public Property Property { get; }
}

public class TenantRegisteredEvent : BaseEvent
{
    public TenantRegisteredEvent(Tenant tenant, Property property)
    {
        Tenant = tenant;
        Property = property;
    }

    public Tenant Tenant { get; }
    public Property Property { get; }
}

public class SuperintendentChangedEvent : BaseEvent
{
    public SuperintendentChangedEvent(Property property, ValueObjects.PersonContactInfo oldSuperintendent, ValueObjects.PersonContactInfo newSuperintendent)
    {
        Property = property;
        OldSuperintendent = oldSuperintendent;
        NewSuperintendent = newSuperintendent;
    }

    public Property Property { get; }
    public ValueObjects.PersonContactInfo OldSuperintendent { get; }
    public ValueObjects.PersonContactInfo NewSuperintendent { get; }
}

public class UnitAddedEvent : BaseEvent
{
    public UnitAddedEvent(Property property, string unitNumber)
    {
        Property = property;
        UnitNumber = unitNumber;
    }

    public Property Property { get; }
    public string UnitNumber { get; }
}

public class UnitRemovedEvent : BaseEvent
{
    public UnitRemovedEvent(Property property, string unitNumber)
    {
        Property = property;
        UnitNumber = unitNumber;
    }

    public Property Property { get; }
    public string UnitNumber { get; }
}