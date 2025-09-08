using Demo.RentalRepairs.WebApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.RentalRepairs.WebApi.Swagger.Examples
{
    public class TenantRequestDocModelExample : IExamplesProvider<TenantRequestDocModel>
    {
        public TenantRequestDocModel GetExamples()
        {
            return new TenantRequestDocModel()
            {
                Title = "Power plug in kitchen"
            };
        }
    }
}
