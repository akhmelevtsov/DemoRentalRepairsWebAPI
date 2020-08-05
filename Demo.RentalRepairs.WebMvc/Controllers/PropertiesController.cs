using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.WebApi.Models;
using Demo.RentalRepairs.WebMvc.Framework;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    public class PropertiesController : BaseController
    {
        private readonly IPropertyService _propertyService;

        private readonly IUserAuthorizationService _userAuthCoreService;

        //private readonly 
        public PropertiesController(IPropertyService propertyService, IUserAuthorizationService userAuthCoreService) : base()
        {
            _propertyService = propertyService;
            _userAuthCoreService = userAuthCoreService;
        }
        public IActionResult Index()
        {
            SetUser();

            var list = _propertyService.GetAllProperties().Select(s => s.BuildModel()).ToList();
            return View(list);
        }

        private void SetUser()
        {
            var loggedUser = HttpContext.Session.GetComplexData<UserClaims>("LoggedUser");
            if (loggedUser == null)
            {
                loggedUser = _userAuthCoreService.SetUser(UserRolesEnum.Superintendent, "super@email.com");
                HttpContext.Session.SetComplexData("LoggedUser", loggedUser);
            }
            else
            {
                _userAuthCoreService.SetUser(loggedUser);
            }
        }

        public IActionResult Details(string id)
        {
            SetUser();
            var tenant = _propertyService.GetPropertyByCode( id ).BuildModel();


            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }
        // GET: Movies/Create
        public IActionResult Create()
        {
            SetUser();
            var model =  new PropertyModel()
            {
                //Code = "moonlight",
                //Name = "Moonlight Apartments",
                //Address = new PropertyAddress() { StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                //NoReplyEmailAddress =
                //    "",
                //PhoneNumber = "905-111-1111",
                //Superintendent = new PersonContactInfo()
                //{
                //    EmailAddress = "propertymanagement@moonlightapartments.com",
                //    FirstName = "John",
                //    LastName = "Smith",
                //    MobilePhone = "647-222-5321"
                //},
                //Units = new List<string> { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" },
                //UnitsStr = "11,12,13,14, 21, 22, 23, 24, 31, 32, 33, 34"

            };
            return View(model);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create( PropertyModel  prop)
        {
            SetUser();
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
                await _propertyService.AddPropertyAsync(new AddPropertyCommand(prop.Name, prop.Code, prop.Address, prop.PhoneNumber, prop.Superintendent,
                    prop.Units.ToList(), prop.NoReplyEmailAddress ));
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
               
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View("Create", prop);
            }


           
            return RedirectToAction("Index");
        }

    }
}