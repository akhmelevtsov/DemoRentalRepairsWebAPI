using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class WorkerEditViewModel
    {
        public PersonContactInfo ContactInfo { get; set; } = new PersonContactInfo();

    }
}
