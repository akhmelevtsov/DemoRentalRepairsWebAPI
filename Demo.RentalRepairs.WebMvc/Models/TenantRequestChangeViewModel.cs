using System;
using System.ComponentModel;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class TenantRequestChangeViewModel
    {
        [DisplayName("Date")]
        public DateTime UpdateDate { get; set;  }
        [DisplayName("Status")]
        public string  TenantRequestStatus { get; set; }
        [DisplayName("Comments")]
        public string  Comments { get; set; }
 
    }
}
