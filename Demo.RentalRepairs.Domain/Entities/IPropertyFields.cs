using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public interface IPropertyFields
    {
        string Name { get; }
        string Code { get; }
        PropertyAddress Address { get; }
        string PhoneNumber { get; }
        PersonContactInfo Superintendent { get; }
        string NoReplyEmailAddress { get; set; }
        List<string> Units { get; }
    }
}