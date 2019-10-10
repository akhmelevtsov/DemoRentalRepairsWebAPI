using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IPropertyService
    {
        IEnumerable<Property> GetAllProperties();
        Property AddProperty(string name, string code, PropertyAddress propertyAddress, string phoneNumber,
            PersonContactInfo superintendentInfo, List<string> units);
        Property GetPropertyByCode(string propCode);

        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);
        Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber);

        IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit);
        TenantRequest RegisterTenantRequest(string propCode, string tenantUnit , TenantRequestDoc tenantRequestDoc);
        TenantRequest ChangeRequestStatus(string propCode, string tenantUnit,string requestCode, TenantRequestStatusEnum newStatus,TenantRequestBaseDoc tenantRequestBaseDoc);
        //TenantRequest ChangeRequestStatus(Guid tenantRequestId, TenantRequestStatusEnum newStatus, TenantRequestBaseDoc tenantRequestBaseDoc);
        Tenant GetTenantByPropertyUnit(string propCode, string tenantCode);
    }
}