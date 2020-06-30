using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class NotifyPartiesServiceMock : INotifyPartiesService
    {
     

        public EmailInfo CreateAndSendEmail(TenantRequest tenantRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
