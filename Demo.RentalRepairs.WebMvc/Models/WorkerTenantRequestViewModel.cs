using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class WorkerTenantRequestViewModel
    {
        [DisplayName("N")]
        public string RequestCode { get; set; }
        [DisplayName("Status")]
        public string RequestStatus { get; set; }
        [DisplayName("Created")]
        public DateTime DateCreated { get; set; }
        [DisplayName("Unit")]
        public string UnitNumber { get; set; }

        public bool ShowScheduleRejectButtons { get; set; }
        [DisplayName("Title")]
        public string RequestTitle { get; set; }
        [DisplayName("Description")]
        public string RequestDescription { get; set; }

        public bool ShowCloseButton { get; set; }
       

        public DateTime ScheduledDate { get; set; }
        public bool ShowReportButton { get; set; }
    }
}
