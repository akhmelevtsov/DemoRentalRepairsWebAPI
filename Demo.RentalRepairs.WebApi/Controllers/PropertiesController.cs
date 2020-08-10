using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.WebApi.Models;
using Demo.RentalRepairs.WebApi.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        // GET api/v1/properties
        /// <summary>
        /// Retrieves a list of properties 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PropertyModel>> Get()
        {
            var list = _propertyService.GetAllProperties().Select( s=> s.BuildModel() ).ToList() ;
            return list ;
        }

        // GET api/v1/properties/code
        /// <summary>
        /// Retrieves property details 
        /// </summary>

        [HttpGet("{propCode}",Name = "GetByCode")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public ActionResult<PropertyModel> Get([FromRoute ]string propCode)
        {
            var property = _propertyService.GetPropertyByCode(propCode);
            return property.BuildModel();
        }

        // POST api/v1/properties
        /// <summary>
        /// Creates a new property 
        /// </summary>
        [HttpPost]
      
        [SwaggerRequestExample(typeof(PropertyModel), typeof(PropertyModelExample))]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public void Post([BindRequired, FromBody] PropertyModel prop)
        {
            
            //var validator = new PropertyValidator();

            //var results = validator.Validate(prop);
            //if (!results.IsValid)
            //{
            //    throw new DomainValidationException("create_property_validation", results.Errors);
            //}
            _propertyService.AddPropertyAsync(new RegisterPropertyCommand(prop.Name, prop.Code, prop.Address, prop.PhoneNumber, prop.Superintendent, prop.Units.ToList(), prop.NoReplyEmailAddress ) );
        }
        
    }
}
