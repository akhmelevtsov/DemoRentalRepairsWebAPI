using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IPropertyService
    {
        IEnumerable<Property> GetAllProperties();                                                                   //anonymous
        Property AddProperty(string name, string code, PropertyAddress propertyAddress, string phoneNumber,     
            PersonContactInfo superintendentInfo, List<string> units);                                              // owner
        Property GetPropertyByCode(string propCode);                                                                //anonymous

        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);                                                // owner , super       
        Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber);                    //tenant

        IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit);                         //tenant, owner, super
        TenantRequest RegisterTenantRequest(string propCode, string tenantUnit , TenantRequestDoc tenantRequestDoc);   //tenant
        TenantRequest ChangeRequestStatus(string propCode, string tenantUnit,string requestCode, 
            TenantRequestStatusEnum newStatus,TenantRequestBaseDoc tenantRequestBaseDoc);                             //super, worker      
       
        Tenant GetTenantByPropertyUnit(string propCode, string tenantCode);  // owner, super
    }
}