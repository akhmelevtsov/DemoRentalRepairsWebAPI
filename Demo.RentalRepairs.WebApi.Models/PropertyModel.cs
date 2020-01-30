using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace Demo.RentalRepairs.WebApi.Models
{
    public class PropertyModel : IProperty
    {
        //[Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public PropertyAddress Address { get; set; }
        public string PhoneNumber { get; set; }
        public PersonContactInfo Superintendent { get; set; }
        public string NoReplyEmailAddress { get; set; }
        [JsonProperty( ObjectCreationHandling = ObjectCreationHandling.Auto )]
        public List<string> Units { get;  set; }
        public string UnitsStr { get; set;  }

        
    }
}
