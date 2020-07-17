using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface  IPropertyRepository
    {
        IEnumerable<Property> GetAllProperties();
        void AddProperty(Property prop);
        Property GetPropertyByCode(string propertyCode);

        void AddTenant(Tenant tenant);
        Tenant GetTenantByUnitNumber(string tenantCode, string propCode);
        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);

        IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId);
        Property  FindPropertyByLoginEmail(string emailAddress);
        void AddTenantRequest(TenantRequest tTenantRequest);
        TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode);
        TenantRequest GetTenantRequestById(Guid tenantRequestId);
        void UpdateTenantRequest(TenantRequest tTenantRequest);

        Tenant FindTenantByLoginEmail(string emailAddress);
        Worker FindWorkerByLoginEmail(string emailAddress);
        void AddWorker(Worker worker);
        IEnumerable<Worker> GetAllWorkers();
    }
}
