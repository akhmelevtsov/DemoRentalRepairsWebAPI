namespace RentalRepairs.Domain.Common;

public interface IAggregateRoot
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    void ClearDomainEvents();
}