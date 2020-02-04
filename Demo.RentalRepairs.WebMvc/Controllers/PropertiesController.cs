using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.WebApi.Models;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;
        //private readonly 
        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        public IActionResult Index()
        {
            var list = _propertyService.GetAllProperties().Select(s => s.BuildModel()).ToList();
            return View(list);
        }
        // GET: Movies/Create
        public IActionResult Create()
        {
            var model =  new PropertyModel()
            {
                Code = "moonlight",
                Name = "Moonlight Apartments",
                Address = new PropertyAddress() { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                NoReplyEmailAddress =
                    "",
                PhoneNumber = "905-111-1111",
                Superintendent = new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "647-222-5321"
                },
                Units = new List<string> { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" },
                UnitsStr = "11,12,13,14, 21, 22, 23, 24, 31, 32, 33, 34"

            };
            return View(model);
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( PropertyModel  prop)
        {
            if (!ModelState.IsValid)
            { // re-render the view when validation failed.
                return View("Create", prop);
            }

            prop.Units = new List<string>();

            if (!string.IsNullOrEmpty(prop.UnitsStr))
            {
                var arr = prop.UnitsStr.Split(',');
                prop.Units = arr.ToList().Select(x => string.IsNullOrEmpty(x) ? "": x.Trim()).ToList() ;
            }

            try
            {
                _propertyService.AddProperty(prop.Name, prop.Code, prop.Address, prop.PhoneNumber, prop.Superintendent,
                    prop.Units.ToList());
            }
            catch (DomainValidationException vex)
            {
                ModelState.Clear();
                foreach (var err in vex.Errors)
                {
                    var propName = err.PropertyName.ToString();
                    if (propName.StartsWith("Units") && propName != "UnitsStr")
                        propName = "UnitsStr";
                    ModelState.AddModelError(propName, err.ErrorMessage);

                }

                return View("Create", prop);
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);
                return View("Create", prop);
                // ModelState.AddModelError
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View("Create", prop);
            }


            //var service = new ValidationRulesService();

            //var result = service.ValidateProperty(prop);

            //if (result.IsValid == false)
            //{
               

            //}
            //ModelState.ClearValidationState();
            //ModelState.AddModelError("Name", "Bad name");
            //return View(prop);
           
            //_propertyService.AddProperty(prop.Name, prop.Code, prop.Address, prop.PhoneNumber, prop.Superintendent, prop.Units.ToList());
            //TempData["notice"] = "Person successfully created";
            return RedirectToAction("Index");
        }

    }
}