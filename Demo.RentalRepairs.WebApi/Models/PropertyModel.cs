using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.WebApi.Models
{
    public class PropertyModel 
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public PropertyAddress Address { get; set; }
        public string PhoneNumber { get; set; }
        public PersonContactInfo Superintendent { get; set; }
        public string NoReplyEmailAddress { get; set; }
        [JsonProperty( ObjectCreationHandling = ObjectCreationHandling.Auto )]
        public List<string> Units { get; internal set; }

        
    }
}
