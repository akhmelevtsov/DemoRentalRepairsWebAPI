using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Events;

namespace RentalRepairs.Domain.Entities;

public class Tenant : BaseEntity
{
    private readonly List<TenantRequest> _requests = new();

    protected Tenant() 
    { 
        // For EF Core
        ContactInfo = null!;
        PropertyCode = string.Empty;
        UnitNumber = string.Empty;
        Property = null!;
    }

    public Tenant(Property property, PersonContactInfo contactInfo, string unitNumber)
    {
        if (property == null)
            throw new ArgumentNullException(nameof(property));
        
        if (contactInfo == null)
            throw new ArgumentNullException(nameof(contactInfo));
        
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("Unit number cannot be empty", nameof(unitNumber));

        Property = property;
        PropertyCode = property.Code;
        ContactInfo = contactInfo;
        UnitNumber = unitNumber;
        RequestsCount = 0;
    }

    public PersonContactInfo ContactInfo { get; private set; } = null!;
    public string PropertyCode { get; private set; } = string.Empty;
    public string UnitNumber { get; private set; } = string.Empty;
    public Property Property { get; private set; } = null!;
    public int RequestsCount { get; private set; }

    public IReadOnlyCollection<TenantRequest> Requests => _requests.AsReadOnly();

    public TenantRequest CreateRequest(string title, string description, string urgencyLevel)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Request title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Request description cannot be empty", nameof(description));

        RequestsCount++;
        var requestCode = $"{PropertyCode}-{UnitNumber}-{RequestsCount:D4}";

        var tenantRequest = new TenantRequest(
            this,
            requestCode,
            title,
            description,
            urgencyLevel);

        _requests.Add(tenantRequest);

        return tenantRequest;
    }

    public void UpdateContactInfo(PersonContactInfo newContactInfo)
    {
        if (newContactInfo == null)
            throw new ArgumentNullException(nameof(newContactInfo));

        var oldContactInfo = ContactInfo;
        ContactInfo = newContactInfo;

        // Domain event for contact info change
        Property.AddDomainEvent(new TenantContactInfoChangedEvent(this, oldContactInfo, newContactInfo));
    }
}