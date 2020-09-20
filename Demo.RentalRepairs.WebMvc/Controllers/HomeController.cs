using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Demo.RentalRepairs.WebMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUserAuthorizationService _userAuthCoreService;
      
        public HomeController(IUserAuthorizationService userAuthCoreService)
        {
            _userAuthCoreService = userAuthCoreService;
        }
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User);
            return View(loggedUser);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AfterSignOut()
        {
            if (TempData["JustEnrolled"] != null)
            {
                TempData["JustEnrolled"] = null;
                return RedirectToAction("SignIn", "Account", new {area = "AzureADB2C"});
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
    }
}
