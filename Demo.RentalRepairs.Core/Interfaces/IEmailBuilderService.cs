using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IEmailBuilderService
    {
        EmailInfo CreateTenantRequestEmail(TenantRequest tenantRequest, Worker worker);
    }
}