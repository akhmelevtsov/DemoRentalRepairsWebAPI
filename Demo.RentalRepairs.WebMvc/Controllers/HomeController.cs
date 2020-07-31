﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Demo.RentalRepairs.WebMvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public HomeController()
        {
           
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
