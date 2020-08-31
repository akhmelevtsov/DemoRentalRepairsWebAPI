using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IPropertyRepository
    {
        void AddWorker(Worker worker);
        void AddProperty(Property prop);
        void AddTenant(Tenant tenant);
        void AddTenantRequest(TenantRequest tTenantRequest);
        void UpdateTenantRequest(TenantRequest tTenantRequest);

        IEnumerable<Property> GetAllProperties();
        Property GetPropertyByCode(string propertyCode);
        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);
        IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode);

        Tenant GetTenantByUnitNumber(string tenantCode, string propCode);
        IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId);
        TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode);

        IEnumerable<Worker> GetAllWorkers();
        IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail);
        Worker GetWorkerByEmail(string email);
    }
}
