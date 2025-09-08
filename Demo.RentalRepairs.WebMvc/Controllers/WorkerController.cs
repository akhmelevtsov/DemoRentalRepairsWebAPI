using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.WebMvc.Interfaces;
using Demo.RentalRepairs.WebMvc.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    [Authorize]
    public class WorkerController : Controller
    {
        private readonly ValidationRulesService _validationRulesService = new ValidationRulesService();
        private readonly IPropertyService _propertyService;
        private readonly IUserAuthorizationService _authService;
        private readonly IIdentityRedirectionService _redirectionService;

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

        public WorkerController(IPropertyService propertyService, IUserAuthorizationService userAuthCoreService, IIdentityRedirectionService redirectionService
        )
        {
            _propertyService = propertyService;
            _authService = userAuthCoreService;
            _redirectionService = redirectionService;
        }
        
        [Authorize(Policy = "RequireWorkerRole")]
        public async Task<IActionResult> Requests()
        {
            // Authorization is now handled by the policy, no need for manual checks
            var userEmail = User.Identity?.Name;
            var worker = _propertyService.GetWorkerByEmail(userEmail);
            
            if (worker == null)
            {
                return RedirectToAction(nameof(Register));
            }

            var requests = _propertyService.GetWorkerRequests(userEmail)
                .OrderByDescending(s => s.DateCreated)
                .Select(s => s.BuildViewModel())
                .ToList();

            return View(requests);
        }

        // GET: Worker/Register
        [AllowAnonymous] // Allow registration for authenticated but unassigned users
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Only allow registration if user is authenticated but not yet assigned a role
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.Identity.Name;
            var existingWorker = _propertyService.GetWorkerByEmail(userEmail);
            
            if (existingWorker != null)
            {
                return RedirectToAction(nameof(Requests));
            }

            var viewModel = new WorkerEditViewModel();
            viewModel.ContactInfo.EmailAddress = userEmail;
            return View(viewModel);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(WorkerEditViewModel worker)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // re-render the view when validation failed.
                return View("Register", worker);
            }

            try
            {
                await _propertyService.RegisterWorkerAsync(worker.ContactInfo);
                return _redirectionService.RedirectAfterEnrollment(TempData, this);
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
        }

        /// <summary>
        /// ReportRequest
        /// </summary>
        [Authorize(Policy = "RequireWorkerRole")]
        [HttpGet]
        public async Task<IActionResult> ReportRequest(string propCode, string unitNumber, string requestCode)
        {
            try
            {
                var vm = await GetTenantRequestViewModel(propCode, unitNumber, requestCode);
                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }
        
        [Authorize(Policy = "RequireWorkerRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportRequest(ReportRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _propertyService.ExecuteTenantRequestCommandAsync(
                    model.PropertyCode, 
                    model.UnitNumber, 
                    model.RequestCode, 
                    new ReportServiceWorkCommand()
                    { 
                        Notes = model.ReportNotes, 
                        Success = model.Success 
                    });

                return RedirectToAction(nameof(Requests));
            }
            catch (DomainValidationException vex)
            {
                MapValidationErrorsToModel(vex);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        
        [Authorize(Policy = "RequireWorkerRole")]
        [HttpGet]
        public async Task<IActionResult> RequestDetails(string propCode, string unitNumber, string requestCode)
        {
            try
            {
                var vm = await GetTenantRequestViewModel(propCode, unitNumber, requestCode);
                return View(vm);
            }
            catch (CoreAuthorizationException)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        #region Private Methods
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
        
        private async Task<PropertyTenantRequestViewModel> GetTenantRequestViewModel(string propCode, string unitNumber, string requestCode)
        {
            await Task.CompletedTask;

            var tenantRequest = _propertyService.GetTenantRequest(propCode, unitNumber, requestCode);
            var vm = tenantRequest.BuildViewModel();
            return vm;
        }
        #endregion
    }
}