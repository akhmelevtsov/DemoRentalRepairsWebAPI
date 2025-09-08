using System.ComponentModel;

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
