using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IPropertyService
    {
       
        IEnumerable<Property> GetAllProperties();                                                                   
   
        Task<Worker> AddWorkerAsync(PersonContactInfo workerContactInfo);
        Task<Property> AddPropertyAsync(AddPropertyCommand propertyInfo);                                                             
        Task<Tenant> AddTenantAsync(string propertyCode, PersonContactInfo contactInfo, string unitNumber);    
        Task<TenantRequest> RegisterTenantRequestAsync(string propCode, string tenantUnit , RegisterTenantRequestCommand tenantRequestDoc);  
        Task<TenantRequest> ExecuteTenantRequestCommandAsync(string propCode, string tenantUnit,string requestCode, 
            ITenantRequestCommand tenantRequestBaseDoc);     
        
        Property GetPropertyByCode(string propCode);                                                               
        IEnumerable<Tenant> GetPropertyTenants(string propertyCode);                                                 
        IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode);

        IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit);                        

        TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode);
        Tenant GetTenantByPropertyUnit(string propCode, string tenantCode);  // owner, super
        IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail);
        IEnumerable<Worker> GetAllWorkers();
        Worker GetWorkerByEmail(string email);
    }
}