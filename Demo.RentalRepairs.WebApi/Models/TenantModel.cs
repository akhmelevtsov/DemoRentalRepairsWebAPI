using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.WebApi.Models
{
    public class TenantModel
    {
        public PersonContactInfo ContactInfo { get; set; }
        public string UnitNumber { get; set; }
       
    }
}
