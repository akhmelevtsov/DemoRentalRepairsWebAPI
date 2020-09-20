using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    public class AdB2CAccountController : Controller
    {
        private readonly IUserAuthorizationService _userAuthCoreService;

        public AdB2CAccountController(IUserAuthorizationService userAuthCoreService)
        {
            _userAuthCoreService = userAuthCoreService;
        }
        //[HttpGet]
        //public IActionResult SignIn()
        //{
        //    var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
        //    return Challenge(
        //        new AuthenticationProperties { RedirectUri = redirectUrl },
        //        OpenIdConnectDefaults.AuthenticationScheme);
        //}

        //[HttpGet]
        //public IActionResult SignOut()
        //{
        //    var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
        //    return SignOut(
        //        new AuthenticationProperties { RedirectUri = callbackUrl },
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        OpenIdConnectDefaults.AuthenticationScheme);
        //}

        [HttpGet]
        public IActionResult SignedOut()
        {
            //await Task.CompletedTask;
            //var loggedUser = await _userAuthCoreService.GetUserClaims(User);
            //if  (loggedUser.UserRole != UserRolesEnum.Anonymous )
            //     return RedirectToAction("SignedOut", "Account", new { area = "AzureADB2C" });

            //if (TempData ["JustEnrolled"] != null)
            //       return RedirectToAction("SignIn", "Account", new { area = "AzureADB2C" });
            return View();

            //if (User.Identity.IsAuthenticated)//{
            //    // Redirect to home page if the user is authenticated.
            //    return RedirectToAction(nameof(HomeController.Index), "Home");
            //}

            //return RedirectToAction(nameof(HomeController.Index), "ThePathYouWant");
        }

        //[HttpGet]
        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}
    }
}