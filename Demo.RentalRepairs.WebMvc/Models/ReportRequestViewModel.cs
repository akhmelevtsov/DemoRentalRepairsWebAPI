using System.ComponentModel.DataAnnotations;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public class ReportRequestViewModel
    {
        [Required]
        [StringLength(50)]
        public string PropertyCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string UnitNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string RequestCode { get; set; } = string.Empty;

        [StringLength(1000)]
        public string ReportNotes { get; set; } = string.Empty;

        public bool Success { get; set; }
    }
}