using System.ComponentModel.DataAnnotations;

namespace Demo.RentalRepairs.WebMvc.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
