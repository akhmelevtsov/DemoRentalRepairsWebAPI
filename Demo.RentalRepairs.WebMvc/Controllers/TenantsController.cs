using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.WebApi.Models;
using Demo.RentalRepairs.WebMvc.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    public class TenantsController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ValidationRulesService _validationRulesService = new ValidationRulesService();
        private readonly Dictionary<string, string> _vDict = new Dictionary<string, string>()
        {
            {"FirstName", "ContactInfo.FirstName"},
            {"LastName", "ContactInfo.LastName"},
            {"MobilePhone", "ContactInfo.MobilePhone"},
            {"EmailAddress", "ContactInfo.EmailAddress"},
            {"Code", "SelectedPropertyCode"},
            {"UnitNumber", "SelectedUnitNumber"},
        };

        public TenantsController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }


        public IActionResult Index()
        {
            

            List<TenantModel> list = null;
            //try
            //{
                list = _propertyService.GetPropertyTenants("moonlight").Select(s => s.BuildModel()).ToList();
            //}
            //catch (DomainAuthorizationException ex1)
            //{
            //   //
            //}

            return View(list);
        }

       
        public IActionResult Details(string propCode, string unitNumber)
        {

            var tenant = _propertyService.GetTenantByPropertyUnit(propCode, unitNumber).BuildModel() ;

         
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        [HttpGet]
        public ActionResult GetUnits(string propCode)
        {
            var validationResult = _validationRulesService.ValidatePropertyCode(propCode);

            if (validationResult.IsValid)
            {
                var units = GetUnitListItems(propCode);
                return Json(units);
            }
            return null;
        }
        // GET: Tenants/Register
        public IActionResult Register()
        {
           
            
            var list = GetPropertyList();

            var viewModel = new TenantEditViewModel()
            {
               
                PropertyList = list
            };


            

            return View(viewModel);
        }

 

        // POST: Tenants/Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(TenantEditViewModel tenant)
        {

            tenant.PropertyList = GetPropertyList();
            tenant.UnitList = GetUnitListItems(tenant.SelectedPropertyCode);

            if (!ModelState.IsValid)
            { // re-render the view when validation failed.
                return View("Register", tenant);
            }

            var propCode = tenant.SelectedPropertyCode == _selectPropertyTip ? "" : tenant.SelectedPropertyCode;
            var unitNumber =  (tenant.SelectedUnitNumber == null || tenant.SelectedUnitNumber.StartsWith("--")) ? "" : tenant.SelectedUnitNumber; 
            try
            {
                await _propertyService.RegisterTenantAsync(propCode, tenant.ContactInfo, unitNumber);
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);

                return View(tenant);
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);
                return View("Register", tenant);
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Register", tenant);
            }
            return RedirectToAction("Index");
        }

        readonly string _selectPropertyTip = "-- select property --";
        readonly string _selectUnitTip = "-- select unit --";
        private List<SelectListItem> GetPropertyList()
        {
            var list = _propertyService.GetAllProperties()
                .Select(x => new SelectListItem() { Value = x.Code, Text = x.Name }).ToList();
            var propertyTip = new SelectListItem()
            {
                Value = null,
                Text = _selectPropertyTip
            };
            list.Insert(0, propertyTip);
            return list;
        }
        private List<SelectListItem> GetUnitListItems(string propCode)
        {
            
            if (string.IsNullOrEmpty( propCode)  || propCode == _selectPropertyTip)
                return new List<SelectListItem>();

            var property = _propertyService.GetPropertyByCode(propCode);

            var units = property.Units.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
            var propertyTip = new SelectListItem()
            {
                Value = null,
                Text = _selectUnitTip
            };
            units.Insert(0, propertyTip);
            return units;
        }

        private void MapValidationErrorsToModel(ValidationException vex)
        {
            //ModelState.Clear();
            foreach (var err in vex.Errors)
            {
                var propName = err.PropertyName.ToString();

                if (!_vDict.ContainsKey(propName ))
                    throw new ApplicationException("Internal error");
                propName = _vDict[propName];
                //if (!ModelState.Keys.Contains(propName))
                //    throw new ApplicationException("Internal error");

              
                ModelState.AddModelError(propName, err.ErrorMessage);
            }
        }
    }

  
}