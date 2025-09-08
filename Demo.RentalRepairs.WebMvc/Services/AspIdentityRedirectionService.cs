using Demo.RentalRepairs.WebMvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Demo.RentalRepairs.WebMvc.Services
{
    public class AspIdentityRedirectionService : IIdentityRedirectionService
    {
        public RedirectToActionResult RedirectAfterEnrollment(ITempDataDictionary tempData, Controller controller)
        {
            return controller.RedirectToAction("Requests");
        }
    }
}
