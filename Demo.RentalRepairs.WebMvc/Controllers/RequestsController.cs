using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IPropertyService _propertyService;

        public RequestsController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        public IActionResult Index()
        {
            var requests = _propertyService.GetTenantRequests("moonlight", "21").Select(s => s.BuildModel()).ToList();
            return View(requests);
        }
    }
}