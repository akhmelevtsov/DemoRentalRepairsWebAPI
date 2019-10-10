using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Models.Examples
{
    public class PropertyModelExample : IExamplesProvider 
    {
        public object GetExamples()
        {

            return new PropertyModel()
            {
                Code = "moonlight", Name = "Moonlight Apartments",
                Address = new PropertyAddress() {StreetNumber = "1", StreetName = "Moonlight Creek", City = "Toronto", PostalCode = "M95 4T1"}, NoReplyEmailAddress =
                "",
                PhoneNumber = "905-111-1111",
                Superintendent =new PersonContactInfo()
                {
                    EmailAddress = "propertymanagement@moonlightapartments.com",
                    FirstName = "John",
                    LastName = "Smith",
                    MobilePhone = "647-222-5321"
                },
                Units = new List<string> { "11", "12", "13", "14", "21", "22", "23", "24", "31", "32", "33", "34" },
               
            };

        }
    }
}
