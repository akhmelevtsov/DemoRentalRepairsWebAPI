using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Events;
using RentalRepairs.Domain.Exceptions;

namespace RentalRepairs.Domain.Entities;

public class Property : BaseEntity, IAggregateRoot
{
    private readonly List<Tenant> _tenants = new();

    protected Property() 
    { 
        // For EF Core
        Name = string.Empty;
        Code = string.Empty;
        Address = null!;
        PhoneNumber = string.Empty;
        Superintendent = null!;
        Units = new List<string>();
        NoReplyEmailAddress = string.Empty;
    }

    public Property(
        string name,
        string code,
        PropertyAddress address,
        string phoneNumber,
        PersonContactInfo superintendent,
        List<string> units,
        string noReplyEmailAddress)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Property code cannot be empty", nameof(code));

        if (address == null)
            throw new ArgumentNullException(nameof(address));

        if (superintendent == null)
            throw new ArgumentNullException(nameof(superintendent));

        Name = name;
        Code = code;
        Address = address;
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Superintendent = superintendent;
        Units = units ?? new List<string>();
        NoReplyEmailAddress = noReplyEmailAddress ?? throw new ArgumentNullException(nameof(noReplyEmailAddress));

        AddDomainEvent(new PropertyRegisteredEvent(this));
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public PropertyAddress Address { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = string.Empty;
    public PersonContactInfo Superintendent { get; private set; } = null!;
    public List<string> Units { get; private set; } = new();
    public string NoReplyEmailAddress { get; private set; } = string.Empty;

    public IReadOnlyCollection<Tenant> Tenants => _tenants.AsReadOnly();

    public Tenant RegisterTenant(PersonContactInfo contactInfo, string unitNumber)
    {
        if (contactInfo == null)
            throw new ArgumentNullException(nameof(contactInfo));

        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number cannot be empty", nameof(unitNumber));

        if (!Units.Contains(unitNumber))
            throw new PropertyDomainException($"Unit {unitNumber} does not exist in property {Code}");

        if (_tenants.Any(t => t.UnitNumber == unitNumber))
            throw new PropertyDomainException($"Unit {unitNumber} is already occupied in property {Code}");

        var tenant = new Tenant(this, contactInfo, unitNumber);
        _tenants.Add(tenant);

        AddDomainEvent(new TenantRegisteredEvent(tenant, this));

        return tenant;
    }

    public void UpdateSuperintendent(PersonContactInfo superintendent)
    {
        if (superintendent == null)
            throw new ArgumentNullException(nameof(superintendent));

        var oldSuperintendent = Superintendent;
        Superintendent = superintendent;

        AddDomainEvent(new SuperintendentChangedEvent(this, oldSuperintendent, superintendent));
    }

    public void AddUnit(string unitNumber)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number cannot be empty", nameof(unitNumber));

        if (Units.Contains(unitNumber))
            throw new PropertyDomainException($"Unit {unitNumber} already exists in property {Code}");

        Units.Add(unitNumber);
        AddDomainEvent(new UnitAddedEvent(this, unitNumber));
    }

    public void RemoveUnit(string unitNumber)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number cannot be empty", nameof(unitNumber));

        if (!Units.Contains(unitNumber))
            throw new PropertyDomainException($"Unit {unitNumber} does not exist in property {Code}");

        if (_tenants.Any(t => t.UnitNumber == unitNumber))
            throw new PropertyDomainException($"Cannot remove unit {unitNumber} as it is currently occupied");

        Units.Remove(unitNumber);
        AddDomainEvent(new UnitRemovedEvent(this, unitNumber));
    }
}