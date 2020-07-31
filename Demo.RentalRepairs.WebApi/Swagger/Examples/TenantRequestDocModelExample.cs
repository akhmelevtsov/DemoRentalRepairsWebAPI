using Demo.RentalRepairs.WebApi.Models;
using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Swagger.Examples
{
    public class TenantRequestDocModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new TenantRequestDocModel()
            {
                Title = "Power plug in kitchen"
            };
        }
    }
}
