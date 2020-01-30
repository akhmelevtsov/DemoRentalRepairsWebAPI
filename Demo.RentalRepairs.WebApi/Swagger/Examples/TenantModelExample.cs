using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.WebApi.Models;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Swagger.Examples
{
    public class TenantModelExample : IExamplesProvider
    {
       
        public object GetExamples()
        {
            return new TenantModel
            {
                UnitNumber = "21",
                
                ContactInfo = new PersonContactInfo()
                {
                    EmailAddress = "tenant123@hotmail.com", FirstName = "John", LastName = "Tenant",
                    MobilePhone = "222-222-2222"
                }
            };
        }
    }
}
