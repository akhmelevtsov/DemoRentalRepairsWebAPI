using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Worker :Entity
    {
        public PersonContactInfo PersonContactInfo { get; set;  }
    }
}
