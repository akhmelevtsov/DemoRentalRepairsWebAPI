using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface INotifyPartiesService
    {
        Task CreateAndSendEmailAsync(TenantRequest tenantRequest);
        //Task CreateAndSendEmailAsync(TenantRequest tenantRequest, Worker worker);
        //Task<EmailInfo> CreateTenantRequestEmailAsync(TenantRequest tenantRequest, Worker worker);
    }
}
