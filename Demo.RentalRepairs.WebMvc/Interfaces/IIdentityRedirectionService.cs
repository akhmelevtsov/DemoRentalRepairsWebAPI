using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Demo.RentalRepairs.WebMvc.Interfaces
{
    public interface IIdentityRedirectionService
    {
        RedirectToActionResult RedirectAfterEnrollment(ITempDataDictionary tempData, Controller controller);
    }
}