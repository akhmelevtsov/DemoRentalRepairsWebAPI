using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalRepairs.WebUI.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        // Clear authentication cookie
        await HttpContext.SignOutAsync();
        
        TempData["Success"] = "You have been logged out successfully.";
        return RedirectToPage("/Index");
    }
}