using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class NotifyPartiesServiceMock : INotifyPartiesService
    {
     

        public Task CreateAndSendEmailAsync(TenantRequest tenantRequest)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAndSendEmailAsync(TenantRequest tenantRequest, Worker worker)
        {
            throw new System.NotImplementedException();
        }

        public Task<EmailInfo> CreateTenantRequestEmailAsync(TenantRequest tenantRequest, Worker worker)
        {
            throw new System.NotImplementedException();
        }
    }
}
