using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Demo.RentalRepairs.WebMvc.Interfaces;
using Demo.RentalRepairs.WebMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    [Authorize]
    public class SuperintendentController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IUserAuthorizationService _authService;
        private readonly IIdentityRedirectionService _redirectionService;


        public SuperintendentController(IPropertyService propertyService, IUserAuthorizationService userAuthCoreService, IIdentityRedirectionService redirectionService

            )
        {
            _propertyService = propertyService;
            _authService = userAuthCoreService;
            _redirectionService = redirectionService;
        }
        [Authorize(Policy = "RequireSuperintendentRole")]
        public async Task<IActionResult> Requests()
        {
            var loggedUser = await _authService.GetUserClaims(User);

            if (string.IsNullOrEmpty(loggedUser.PropCode))
            {
                return base.RedirectToAction(actionName: nameof(this.Register), controllerName: "Superintendent");
            }

            try
            {
                var requests = _propertyService.GetPropertyRequests(loggedUser.PropCode).Select(s => s.BuildViewModel()).OrderByDescending(x => x.DateCreated)
                    .ToList();

                return View(requests);

            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }

        [Authorize(Policy = "RequireAnonymousRole")]
        public async Task<IActionResult> Register()
        {

            var loggedUser = await _authService.GetUserClaims(User);


            var model = new PropertyModel()
            {
                Code = "sunlight",
                Name = "Sun Light Apartments",
                Address = new PropertyAddress()
                { StreetNumber = "1", StreetName = "Sunlight Creek", City = "Toronto", PostalCode = "M9A 4J5" },
                NoReplyEmailAddress = loggedUser.Login,
                PhoneNumber = "905-111-1111",
                Superintendent = new PersonContactInfo()
                {
                    EmailAddress = loggedUser.Login,
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "647-222-5321"
                },
                Units = new List<string> { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" },
                UnitsStr = "11,12,13,14, 21, 22, 23, 24, 31, 32, 33, 34"


            };
            return View(model);
        }

        [Authorize(Policy = "RequireAnonymousRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PropertyModel prop)
        {

            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("Register", prop);
            }

            var loggedUser = await _authService.GetUserClaims(User);

            prop.Units = new List<string>();

            if (!string.IsNullOrEmpty(prop.UnitsStr))
            {
                var arr = prop.UnitsStr.Split(',');
                prop.Units = arr.ToList().Select(x => string.IsNullOrEmpty(x) ? "" : x.Trim()).ToList();
            }

            try
            {
                await _propertyService.RegisterPropertyAsync(new RegisterPropertyCommand(prop.Name, prop.Code, prop.Address,
                    prop.PhoneNumber,
                    prop.Superintendent,
                    prop.Units.ToList(), prop.NoReplyEmailAddress));
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
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

                return View("Register", prop);
            }
            catch (DomainEntityDuplicateException)
            {
                ModelState.AddModelError("Code", "The code is already taken");
                return View("Register", prop);
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);
                return View("Register", prop);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View("Register", prop);
            }


            //return RedirectToAction("Requests", "Superintendent");
            return _redirectionService.RedirectAfterEnrollment(TempData, this);

        }
        /// <summary>
        /// RejectRequest
        /// </summary>
        /// <param name="unitNumber"></param>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpGet]
        public async Task<IActionResult> RejectRequest(string unitNumber, string requestCode)
        {
            var loggedUser = await _authService.GetUserClaims(User);
            try
            {
                var vm = await GetPropertyTenantRequestViewModel(unitNumber, requestCode);
                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(string unitNumber, string requestCode, string rejectNotes)
        {
            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("RejectRequest");
            }
            var loggedUser = await _authService.GetUserClaims(User);

            try
            {
               await  _propertyService.ExecuteTenantRequestCommandAsync(loggedUser.PropCode, unitNumber, requestCode,
                    new RejectTenantRequestCommand() {Notes = rejectNotes});
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }

            return RedirectToAction("Requests", "Superintendent");
        }
        /// <summary>
        /// ScheduleRequest
        /// </summary>
        /// <param name="unitNumber"></param>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpGet]
        public async Task<IActionResult> ScheduleRequest(string unitNumber, string requestCode)
        {
            var loggedUser = await _authService.GetUserClaims(User);
            try
            {
                var vm = await GetPropertyTenantRequestViewModel(unitNumber, requestCode);
                vm.WorkerList = GetWorkerList();


                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }


        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleRequest(PropertyTenantRequestViewModel request, string unitNumber, string requestCode)
        {
           
           var loggedUser = await _authService.GetUserClaims(User);
            if (request.SelectedWorkerEmail == _selectWorkerTip)
            {
                ModelState.Clear();
                ModelState.AddModelError("SelectedWorkerEmail", "Worker should be assigned");
                var vm = await GetPropertyTenantRequestViewModel(unitNumber, requestCode);
                vm.WorkerList = GetWorkerList();
                return View("ScheduleRequest", vm);
            }

            

            try
            {


                //var propCode = tenant.SelectedPropertyCode == _selectPropertyTip ? "" : tenant.SelectedPropertyCode;
                var worker = _propertyService.GetWorkerByEmail(request.SelectedWorkerEmail);
                if (worker == null)
                    throw new Exception("Worker not found");

                await _propertyService.ExecuteTenantRequestCommandAsync(loggedUser.PropCode, unitNumber, requestCode,
                    new ScheduleServiceWorkCommand(worker.PersonContactInfo.EmailAddress, request.ScheduledDate, 1));

                return RedirectToAction("Requests", "Superintendent");
            }
            catch (DomainValidationException vex)
            {
                ModelState.Clear();
                foreach (var err in vex.Errors)
                {
                    var propName = err.PropertyName.ToString();
                    ModelState.AddModelError(propName, err.ErrorMessage);
                }
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("ScheduleRequest", request);

        }
        /// <summary>
        ///  CloseRequest
        /// </summary>
        /// <param name="unitNumber"></param>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpGet]
        public async Task<IActionResult> CloseRequest(string unitNumber, string requestCode)
        {
            var loggedUser = await _authService.GetUserClaims(User);

            try
            {
                var vm = await GetPropertyTenantRequestViewModel(unitNumber, requestCode);


                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseRequest(string unitNumber, string requestCode, string workerId, DateTime serviceDate)
        {
           
            var loggedUser = await _authService.GetUserClaims(User);
            try
            {
                await _propertyService.ExecuteTenantRequestCommandAsync(loggedUser.PropCode, unitNumber, requestCode,
                    new CloseTenantRequestCommand());

                return RedirectToAction("Requests", "Superintendent");
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }
        [Authorize(Policy = "RequireSuperintendentRole")]
        [HttpGet]
        public async Task<IActionResult> RequestDetails(string unitNumber, string requestCode)
        {
            var loggedUser = await _authService.GetUserClaims(User);
            try
            {
                var vm = await GetPropertyTenantRequestViewModel(unitNumber, requestCode);

                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Home");
            }
        }

        #region Private Methods

        private async Task<PropertyTenantRequestViewModel> GetPropertyTenantRequestViewModel(string unitNumber, string requestCode)
        {
            var loggedUser = await _authService.GetUserClaims(User);

            var tenantRequest =
                _propertyService.GetTenantRequest(loggedUser.PropCode, unitNumber, requestCode);


            var vm = tenantRequest.BuildViewModel();
            return vm;
        }
        readonly string _selectWorkerTip = "-- select worker --";

        private List<SelectListItem> GetWorkerList()
        {
            var list = _propertyService.GetAllWorkers()
                .Select(x => new SelectListItem() { Value = x.PersonContactInfo.EmailAddress, Text = x.PersonContactInfo.GetFullName() }).ToList();
            var propertyTip = new SelectListItem()
            {
                Value = null,
                Text = _selectWorkerTip
            };
            list.Insert(0, propertyTip);
            return list;
        }
        #endregion


    }
}