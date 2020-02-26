using System.ComponentModel.DataAnnotations;

namespace Demo.RentalRepairs.WebMvc.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
