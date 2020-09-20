using Demo.RentalRepairs.WebMvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Demo.RentalRepairs.WebMvc.Services
{
    public class AzureAdB2CRedirectionService : IIdentityRedirectionService
    {
        public RedirectToActionResult RedirectAfterEnrollment(ITempDataDictionary tempData, Controller controller)
        {
            tempData["JustEnrolled"] = "yes";

            return controller.RedirectToAction("SignOut", "Account", new { area = "AzureADB2C" });
        }

    }
}
