namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public  class RegisterTenantRequestCommand : ITenantRequestCommand
    {
        public string Title { get; set; }
        public string Description { get; set;  }
        public string Comments()
        {
            return Title;
        }
    }
}
