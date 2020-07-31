using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface  IPropertyRepository
    {
        IEnumerable<Property> GetAllProperties();
        void AddProperty(Property prop);
        void AddTenant(Tenant tenant);
        void AddTenantRequest(TenantRequest tTenantRequest);
        void UpdateTenantRequest(TenantRequest tTenantRequest);
        void AddWorker(Worker worker);


        Property GetPropertyByCode(string propertyCode);

        Tenant GetTenantByUnitNumber(string tenantCode, string propCode);
        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);
        IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId);

        TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode);
        TenantRequest GetTenantRequestById(Guid tenantRequestId);

        Property  FindPropertyByLoginEmail(string emailAddress);
        Tenant FindTenantByLoginEmail(string emailAddress);
        //Worker FindWorkerByLoginEmail(string emailAddress);

        IEnumerable<Worker> GetAllWorkers();
        IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode);
        IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail);
        Worker GetWorkerByEmail(string email);
    }
}
