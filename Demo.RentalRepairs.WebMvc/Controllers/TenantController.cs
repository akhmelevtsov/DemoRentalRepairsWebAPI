using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.WebMvc.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    [Authorize]
    public class TenantController : Controller
    {
        private readonly ValidationRulesService _validationRulesService = new ValidationRulesService();
        private readonly IPropertyService _propertyService;
        private readonly IUserAuthorizationService _userAuthCoreService;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ISecurityService _securityService;

        private readonly Dictionary<string, string> _vDict = new Dictionary<string, string>()
        {
            {"FirstName", "ContactInfo.FirstName"},
            {"LastName", "ContactInfo.LastName"},
            {"MobilePhone", "ContactInfo.MobilePhone"},
            {"EmailAddress", "ContactInfo.EmailAddress"},
            {"Code", "SelectedPropertyCode"},
            {"UnitNumber", "SelectedUnitNumber"},
            {"Title", "Title"},
            {"Description", "Description"}

        };

        public TenantController(IPropertyService propertyService, IUserAuthorizationService userAuthCoreService, UserManager<ApplicationUser> userManager//, ISecurityService securityService
        )
        {
            _propertyService = propertyService;
            _userAuthCoreService = userAuthCoreService;
            //_securityService = securityService;

        }
        [Authorize(Policy = "RequireTenantRole")]
        public async Task<IActionResult> Requests()
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name );

            if (!loggedUser.IsRegisteredTenant())
            {
                return base.RedirectToAction(actionName: nameof(this.Register), controllerName: "Tenant");
            }

            try
            {

                var requests = _propertyService.GetTenantRequests(loggedUser.PropCode, loggedUser.UnitNumber)
                    .OrderByDescending(s => s.DateCreated).Select(s => s.BuildViewModel()).ToList();

                //throw new CoreAuthorizationException("", ""); 
                return View(requests);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
        }


        [Authorize(Policy = "RequireTenantRole")]
        public IActionResult CreateRequest()
        {
            return View(new RegisterTenantRequestCommand());
        }

        [Authorize(Policy = "RequireTenantRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(RegisterTenantRequestCommand requestCommand)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            if (!loggedUser.IsRegisteredTenant())
            {
                return base.RedirectToAction(actionName: nameof(this.Register), controllerName: "Tenant");
            }

            if (!ModelState.IsValid)
            {
                return View(requestCommand);
            }

            try
            {
                await _propertyService.RegisterTenantRequestAsync(loggedUser.PropCode, loggedUser.UnitNumber,
                    new RegisterTenantRequestCommand() { Title = requestCommand.Title, Description = requestCommand.Description });
                return RedirectToAction(nameof(Requests));

            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);

            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(requestCommand);
            }

            return View(requestCommand);
        }



        // GET: Tenant/Register
        [Authorize(Policy = "RequireAnonymousRole")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            try
            {
                var list = GetPropertyList();

                var viewModel = new TenantEditViewModel()
                {

                    PropertyList = list,

                };
                viewModel.ContactInfo.EmailAddress = loggedUser.Login;

                return View(viewModel);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
        }

        // POST: Tenant/Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAnonymousRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(TenantEditViewModel tenantVm)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("Register", tenantVm);
            }

            try
            {
                tenantVm.PropertyList = GetPropertyList();
                tenantVm.UnitList = GetUnitListItems(tenantVm.SelectedPropertyCode);
                var propCode = tenantVm.SelectedPropertyCode == _selectPropertyTip ? "" : tenantVm.SelectedPropertyCode;
                var unitNumber = (tenantVm.SelectedUnitNumber == null || tenantVm.SelectedUnitNumber.StartsWith("--"))
                    ? ""
                    : tenantVm.SelectedUnitNumber;

                await _propertyService.RegisterTenantAsync(propCode, tenantVm.ContactInfo, unitNumber);
                //await _securityService.SetLoggedUserClaims( User.Identity.Name ,UserRolesEnum.Tenant,  propCode, unitNumber);
                return RedirectToAction("Requests", "Tenant");


            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
            catch (DomainEntityDuplicateException)
            {
                ModelState.AddModelError("SelectedUnitNumber", "The Unit is already taken");
                return View("Register", tenantVm);
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);

                return View(tenantVm);
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);
                return View("Register", tenantVm);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Register", tenantVm);
            }


        }
        [Authorize(Policy = "RequireAnonymousRole")]
        [HttpGet]
        public async Task<ActionResult> GetUnits(string propCode)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            var validationResult = _validationRulesService.ValidatePropertyCode(propCode );

            if (validationResult.IsValid)
            {
                var units = GetUnitListItems(propCode);
                return Json(units);
            }
            return null;
        }
        [Authorize(Policy = "RequireTenantRole")]
        [HttpGet]
        public async Task<IActionResult> RequestDetails(string requestCode)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            try
            {
                var tenantRequest =
                    _propertyService.GetTenantRequest(loggedUser.PropCode, loggedUser.UnitNumber, requestCode);


                var vm = tenantRequest.BuildViewModel();

                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
        }
        [Authorize(Policy = "RequireTenantRole")]
        [HttpGet]
        public async Task<ActionResult> RequestHistory(string requestCode)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);

            var tenantRequest =
                _propertyService.GetTenantRequest(loggedUser.PropCode, loggedUser.UnitNumber, requestCode);


            return View(tenantRequest.RequestChanges.BuildViewModel());
        }

        #region Private Methods

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

            if (string.IsNullOrEmpty(propCode) || propCode == _selectPropertyTip)
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
         
            foreach (var err in vex.Errors)
            {
                var propName = err.PropertyName.ToString();

                if (!_vDict.ContainsKey(propName))
                    throw new ApplicationException("Internal error");
                propName = _vDict[propName];
                


                ModelState.AddModelError(propName, err.ErrorMessage);
            }
        }

        #endregion

    }
}