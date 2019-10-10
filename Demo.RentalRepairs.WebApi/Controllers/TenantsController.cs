using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.WebApi.Models;
using Demo.RentalRepairs.WebApi.Models.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/properties/{propertyCode}/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public TenantsController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        // GET api/properties/moonlight/tenants ------------------------------------------------------------
        /// <summary>
        /// Retrieves a list of property tenants
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(TenantModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TenantModel>> Get(string propertyCode)
        {

            var list = _propertyService.GetPropertyTenants(propertyCode).Select(s => s.BuildModel()).ToList();
            return list;
        }

        // GET api/v1/properties/moonlight/tenants/234 ------------------------------------------------------
        /// <summary>
        /// Retrieves tenant details
        /// </summary>

        [HttpGet("{unitNumber}", Name = "GetByUnitNumber")]
        [ProducesResponseType(typeof(TenantModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public ActionResult<TenantModel > Get([FromRoute]string propertyCode, [FromRoute] string unitNumber)
        {
            var tenant = _propertyService.GetTenantByPropertyUnit(propertyCode, unitNumber );
            return tenant.BuildModel();
        }

        // POST api/properties/{propertyCode} -----------------------------------------------------------------
        /// <summary>
        /// Adds new tenant to property
        /// </summary>
        [SwaggerRequestExample(typeof(TenantModel), typeof(TenantModelExample))]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public void Post(string propertyCode,  [FromBody] TenantModel tenant)
        {
            _propertyService.AddTenant(propertyCode, tenant.ContactInfo , tenant.UnitNumber );
        }
    }
}
