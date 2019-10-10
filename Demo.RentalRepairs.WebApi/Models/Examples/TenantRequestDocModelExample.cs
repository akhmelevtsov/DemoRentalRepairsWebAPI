using Swashbuckle.AspNetCore.Examples;

namespace Demo.RentalRepairs.WebApi.Models.Examples
{
    public class TenantRequestDocModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new TenantRequestDocModel()
            {
                RequestItems = new string[] {"Power plug in kitchen", "Water leak in main bathroom"}
            };
        }
    }
}
