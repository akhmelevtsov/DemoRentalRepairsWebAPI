using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.WebMvc.Framework;
using Microsoft.AspNetCore.Mvc;
using Demo.RentalRepairs.WebMvc.Models;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
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
