using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface INotifyPartiesService
    {
        EmailInfo CreateAndSendEmail(TenantRequest tenantRequest);
    }
}
