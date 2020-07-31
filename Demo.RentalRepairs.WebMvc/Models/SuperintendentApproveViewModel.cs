using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class SuperintendentApproveViewModel
    {
        [DisplayName("Title")]
        public string RequestTitle { get; set; }
        [DisplayName("Description")]
        public string RequestDescription { get; set; }
    }
}
