namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class ReportServiceWorkCommand : ITenantRequestCommand
    {
        private ScheduleServiceWorkCommand ServiceWorkOrder { get; set; }
        public bool Success { get; set; }

        public string Notes;


        public string Comments()
        {
            return Notes;
        }
    }
}
