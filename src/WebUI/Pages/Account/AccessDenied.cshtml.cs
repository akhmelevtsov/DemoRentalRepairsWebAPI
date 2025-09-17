using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalRepairs.WebUI.Pages.Account;

public class AccessDeniedModel : PageModel
{
    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }
}