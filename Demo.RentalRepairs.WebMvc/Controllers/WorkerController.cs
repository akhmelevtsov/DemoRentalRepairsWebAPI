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
    [Authorize(Policy = "RequireWorkerRole")]
    public class WorkerController : Controller
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
            {"Description", "Description"},
            {"Notes", "ReportNotes" },
            {"Success", "Success" }

        };

        public WorkerController(IPropertyService propertyService, IUserAuthorizationService userAuthCoreService, UserManager<ApplicationUser> userManager //, ISecurityService securityService
        )
        {
            _propertyService = propertyService;
            _userAuthCoreService = userAuthCoreService;
            //_securityService = securityService;

        }

        public async Task<IActionResult> Requests()
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);

            //_userAuthCoreService.SetUser(loggedUser);
            try
            {
                var worker = _propertyService.GetWorkerByEmail(loggedUser.Login);
                if (worker == null)
                {
                    return base.RedirectToAction(actionName: nameof(this.Register), controllerName: "Worker");
                }


                var requests = _propertyService.GetWorkerRequests(loggedUser.Login)
                    .OrderByDescending(s => s.DateCreated).Select(s => s.BuildViewModel()).ToList();


                return View(requests);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
        }




        // GET: Tenants/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {

            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);

            var viewModel = new WorkerEditViewModel(); // {ContactInfo = {EmailAddress = loggedUser.Login}};
            viewModel.ContactInfo.EmailAddress = loggedUser.Login;
            return View(viewModel);
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(WorkerEditViewModel worker)
        {


            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("Register", worker);
            }
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);

            try
            {
                await _propertyService.RegisterWorkerAsync(worker.ContactInfo);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);

                return View(worker);
            }
            catch (DomainException dex)
            {
                ModelState.AddModelError("", dex.Message);
                return View("Register", worker);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Register", worker);
            }

            //await _securityService.SetLoggedUserClaims( User.Identity.Name , UserRolesEnum.Worker ,"","");

            return RedirectToAction("Requests", "Worker"); //, new { propCode = propCode, unit = unitNumber });
        }


        /// <summary>
        /// ReportRequest
        /// </summary>

        [HttpGet]
        public async Task<IActionResult> ReportRequest(string propCode, string unitNumber, string requestCode)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            try
            {
                var vm = await GetTenantRequestViewModel(propCode, unitNumber, requestCode);
                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportRequest(string propertyCode, string unitNumber, string requestCode, string reportNotes, bool success)
        {

            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("ReportRequest");
            }
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);

            try
            {
               await _propertyService.ExecuteTenantRequestCommandAsync(propertyCode, unitNumber, requestCode, new ReportServiceWorkCommand()
                    { Notes = reportNotes, Success = success });

            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);

                return View("ReportRequest");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("ReportRequest");
            }
            return RedirectToAction("Requests", "Worker");
        }
        [HttpGet]
        public async Task<IActionResult> RequestDetails(string propCode, string unitNumber, string requestCode)
        {
            var loggedUser = await _userAuthCoreService.GetUserClaims(User.Identity.Name);
            try
            {
                var vm = await GetTenantRequestViewModel(propCode, unitNumber, requestCode);
                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return base.RedirectToAction(actionName: "AccessDenied", controllerName: "Account");
            }

        }

        #region Private Methods
        private void MapValidationErrorsToModel(ValidationException vex)
        {
            //ModelState.Clear();
            foreach (var err in vex.Errors)
            {
                var propName = err.PropertyName.ToString();

                if (!_vDict.ContainsKey(propName))
                    throw new ApplicationException("Internal error");
                propName = _vDict[propName];
                //if (!ModelState.Keys.Contains(propName))
                //    throw new ApplicationException("Internal error");


                ModelState.AddModelError(propName, err.ErrorMessage);
            }
        }
        private async Task<PropertyTenantRequestViewModel> GetTenantRequestViewModel(string propCode, string unitNumber, string requestCode)
        {
            //var loggedUser = await _securityService.GetLoggedWorker(User);

            await Task.CompletedTask;

            var tenantRequest =
                _propertyService.GetTenantRequest(propCode, unitNumber, requestCode);


            var vm = tenantRequest.BuildViewModel();
            return vm;
        }
        #endregion


    }
}