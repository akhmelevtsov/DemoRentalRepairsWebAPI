namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class RejectTenantRequestCommand : ITenantRequestCommand
    {
  
        public string Notes { get; set;  }
        public string Comments()
        {
            return Notes;
        }
    }
}
