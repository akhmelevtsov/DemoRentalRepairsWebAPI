namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class ServiceWorkReport : TenantRequestBaseDoc
    {
        private ServiceWorkOrder ServiceWorkOrder { get; set; }
        public string Notes;
   

        //public ServiceWorkReport(ICreatedTimeStamp command, DateTime registerDateTime) : base (command, registerDateTime)
        //{
          
        //}
    }
}
