using System;
using Demo.RentalRepairs.Domain.Entities;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class ScheduleServiceWorkCommand : ITenantRequestCommand
    {
       
        public string WorkerEmailAddress { get; set; }
        public DateTime ServiceDate { get; set; }
        public int WorkOrderNo { get;  }
       

        public ScheduleServiceWorkCommand()
        {

        }
        [JsonConstructor]
        public ScheduleServiceWorkCommand(string  workerEmailAddress, DateTime serviceDate, int workOrderNo)
        {
           
            WorkerEmailAddress = workerEmailAddress;
            ServiceDate = serviceDate;
            WorkOrderNo = workOrderNo;
 
        }

        public string Comments()
        {
            return $"Service date: {ServiceDate.ToShortDateString() }";
        }
    }
}
