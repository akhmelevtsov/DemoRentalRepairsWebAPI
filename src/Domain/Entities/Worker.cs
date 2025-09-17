using RentalRepairs.Domain.Common;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Events;

namespace RentalRepairs.Domain.Entities;

public class Worker : BaseEntity
{
    protected Worker() 
    { 
        // For EF Core
        ContactInfo = null!;
    }

    public Worker(PersonContactInfo contactInfo)
    {
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        IsActive = true;
        AddDomainEvent(new WorkerRegisteredEvent(this));
    }

    public PersonContactInfo ContactInfo { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public string? Specialization { get; private set; }
    public string? Notes { get; private set; }

    public void UpdateContactInfo(PersonContactInfo newContactInfo)
    {
        if (newContactInfo == null)
            throw new ArgumentNullException(nameof(newContactInfo));

        var oldContactInfo = ContactInfo;
        ContactInfo = newContactInfo;

        AddDomainEvent(new WorkerContactInfoChangedEvent(this, oldContactInfo, newContactInfo));
    }

    public void SetSpecialization(string specialization)
    {
        Specialization = specialization;
    }

    public void AddNotes(string notes)
    {
        Notes = string.IsNullOrWhiteSpace(Notes) 
            ? notes 
            : $"{Notes}\n{DateTime.UtcNow:yyyy-MM-dd}: {notes}";
    }

    public void Deactivate(string reason = "")
    {
        IsActive = false;
        AddNotes($"Worker deactivated. Reason: {reason}");
        AddDomainEvent(new WorkerDeactivatedEvent(this, reason));
    }

    public void Activate()
    {
        IsActive = true;
        AddNotes("Worker reactivated");
        AddDomainEvent(new WorkerActivatedEvent(this));
    }
}