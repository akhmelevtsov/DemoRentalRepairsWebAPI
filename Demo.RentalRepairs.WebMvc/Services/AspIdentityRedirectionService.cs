using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
