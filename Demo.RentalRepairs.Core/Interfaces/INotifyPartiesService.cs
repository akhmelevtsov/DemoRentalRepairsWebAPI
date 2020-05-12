using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface INotifyPartiesService
    {
        EmailInfo CreateAndSendEmail(TenantRequest tenantRequest);
    }
}
