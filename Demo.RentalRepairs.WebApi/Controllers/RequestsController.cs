using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.WebApi.Models;
using Demo.RentalRepairs.WebApi.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/properties/{propertyCode}/tenants/{tenantUnit}/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public RequestsController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        // GET api/properties/moonlight/tenants/21/requests/
        /// <summary>
        /// Retrieves a list of tenant requests (tenant)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TenantRequestModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TenantRequestModel>> Get(string propertyCode, string tenantUnit)
        {

            List<TenantRequestModel> list = _propertyService.GetTenantRequests(propertyCode, tenantUnit).Select(s => s.BuildModel()).ToList();
            return list;
        }
        // POST api/properties/moonlight/21/requests 
        /// <summary>
        /// Adds a new tenant request (tenant)
        /// </summary>
        [SwaggerRequestExample(typeof(TenantRequestDocModel), typeof(TenantRequestDocModelExample))]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void Post(string propertyCode, string tenantUnit,  [FromBody] TenantRequestDocModel tenantRequestDocModel )
        {
            _propertyService.RegisterTenantRequestAsync(propertyCode, tenantUnit, tenantRequestDocModel) ;
        }
        // PATCH api/properties/moonlight/tenants/21/requests/1/schedule
        /// <summary>
        /// Schedules work for the tenant request (property owner)
        /// </summary>
        [HttpPatch("schedule",Name = "ScheduleWork")]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void ScheduleWork(string propertyCode, string tenantUnit,string requestCode,  [FromBody] ScheduleServiceWorkCommand  serviceWorkOrder)
        {
            _propertyService.ExecuteTenantRequestCommandAsync(propertyCode,tenantUnit , requestCode, serviceWorkOrder);
        }
        // PATCH api/properties/moonlight/tenants/21/requests/1/reject
        /// <summary>
        /// Declines the request (property owner)
        /// </summary>
        [HttpPatch("{requestCode}/reject", Name = "RejectRequest")]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void RejectRequest(string propertyCode, string tenantUnit, [FromRoute] string requestCode, [FromBody] RejectTenantRequestCommand  tenantRequestRejectNotes)
        {
            _propertyService.ExecuteTenantRequestCommandAsync(propertyCode, tenantUnit, requestCode, tenantRequestRejectNotes);
        }
        // PATCH api/properties/moonlight/tenants/21/requests/1/done
        /// <summary>
        /// Reports work is done (worker)
        /// </summary>
        [HttpPatch("{requestCode}/done", Name = "ReportWorkCompleted")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void ReportWorkCompleted(string propertyCode, string tenantUnit, [FromRoute] string requestCode, [FromBody] ReportServiceWorkCommand serviceWorkReport )
        {
            _propertyService.ExecuteTenantRequestCommandAsync(propertyCode, tenantUnit, requestCode, serviceWorkReport);
        }
        // PATCH api/properties/moonlight/tenants/21/requests/1/failed
        /// <summary>
        /// Reports work can't be completed (worker)
        /// </summary>
        [HttpPatch("{requestCode}/failed", Name = "ReportWorkIncomplete")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void ReportWorkIncomplete(string propertyCode, string tenantUnit, [FromRoute] string requestCode, [FromBody] ReportServiceWorkCommand serviceWorkReport)
        {
            _propertyService.ExecuteTenantRequestCommandAsync(propertyCode, tenantUnit, requestCode, serviceWorkReport);
        }
        // PATCH api/properties/moonlight/tenants/21/requests/1/close
        /// <summary>
        /// Closes the request (property owner)
        /// </summary>
        [HttpPatch("{requestCode}/close", Name = "CloseRequest")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void CloseRequest(string propertyCode, string tenantUnit, [FromRoute] string requestCode)
        {
            _propertyService.ExecuteTenantRequestCommandAsync(propertyCode, tenantUnit, requestCode, null);
        }
     
    }

}
