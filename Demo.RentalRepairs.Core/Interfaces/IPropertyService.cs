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
        //Property AddProperty(string name, string code, PropertyAddress propertyAddress, string phoneNumber,     
        //    PersonContactInfo superintendentInfo, List<string> units);                                              // owner
        Property AddProperty(AddPropertyCommand propertyInfo);                                                             // owner

        Property GetPropertyByCode(string propCode);                                                                //anonymous

        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);                                                // owner , super       

        Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber);    
        //tenant
        IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode);

        IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit);                         //tenant, owner, super
        TenantRequest RegisterTenantRequest(string propCode, string tenantUnit , RegisterTenantRequestCommand tenantRequestDoc);   //tenant
        TenantRequest ExecuteTenantRequestCommand(string propCode, string tenantUnit,string requestCode, 
            ITenantRequestCommand tenantRequestBaseDoc);                             //super, worker      

        TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode);
        Tenant GetTenantByPropertyUnit(string propCode, string tenantCode);  // owner, super
        IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail);
        Worker AddWorker(PersonContactInfo workerContactInfo);
        IEnumerable<Worker> GetAllWorkers();
        Worker GetWorkerByEmail(string email);
    }
}