using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Resources;

namespace Demo.RentalRepairs.Infrastructure
{
    public class TemplateDataService : ITemplateDataService
    {
        public string GetString(string key)
        {
            return MessagesResource.ResourceManager.GetString(key);
        }

       
    }
}
