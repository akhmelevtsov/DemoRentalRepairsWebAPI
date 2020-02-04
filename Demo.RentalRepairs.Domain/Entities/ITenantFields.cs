using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public interface ITenantFields
    {
        PersonContactInfo ContactInfo { get; set; }
        string UnitNumber { get; }
    }
}