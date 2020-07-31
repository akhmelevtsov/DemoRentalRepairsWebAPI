using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class PropertyTenantRequestViewModel
    {
        [DisplayName("N")]
        public string RequestCode { get; set; }
        [DisplayName("Status")]
        public string RequestStatus { get; set; }
        [DisplayName("Date")]
        public DateTime DateCreated { get; set; }
        [DisplayName("Unit N")]
        public string UnitNumber { get; set; }

        public bool ShowScheduleRejectButtons { get; set;  }
        [DisplayName("Subject")]
        public string RequestTitle { get; set; }
        [DisplayName("Description")]
        public string RequestDescription { get; set; }

        public bool ShowCloseButton { get; set; }
        [DisplayName("Notes")]
        public string RejectNotes { get; set;  }

        public List<SelectListItem> WorkerList { get; set; }
       
        public string SelectedWorkerEmail { get; set; }

       
        public DateTime ScheduledDate { get; set; }
        public bool ShowReportButton { get; set; }
        [DisplayName("Notes")]
        public string ReportNotes { get; set; }
      
        public bool Success { get; set; }
        [DisplayName("Property ID")]
        public string PropertyCode { get; set; }
        [DisplayName("History")]
        public List<TenantRequestChangeViewModel> History { get; set; }

        public bool ShowScheduleButton { get; set; }
    }
}
