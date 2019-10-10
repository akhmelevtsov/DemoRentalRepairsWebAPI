using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;

namespace Demo.RentalRepairs.Infrastructure.Repositories
{
    public class PropertyRepositoryInMemory : IPropertyRepository
    {
        private readonly Dictionary< string,Property> _properties = new Dictionary<string, Property>();




        public void AddTenantRequest(TenantRequest tenantRequest)
        {
            // Already added
        }

        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {
            //Already updated
        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            foreach (var kvp in _properties)
            {
                foreach (var tenant in kvp.Value.Tenants)
                {
                    var tenantRequest = tenant.GetActiveRequestById(tenantRequestId);
                    if (tenantRequest != null)
                        return tenantRequest;
                }
            }

            return null;
        }
        
        public IEnumerable<Property> GetAllProperties()
        {
            return _properties.Values.ToList();
        }
        public void AddProperty(Property prop)
        {
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));

            if (_properties.ContainsKey(prop.Code))
                Property.DuplicateException(prop.Code);

            _properties.Add( prop.Code, prop);
        }
        public Property GetPropertyByCode(string propertyCode)
        {
            if (string.IsNullOrEmpty(propertyCode))
                throw new ArgumentNullException(nameof(propertyCode));

            if (!_properties.ContainsKey(propertyCode))
                    Property.NotFoundException(propertyCode);
            return _properties[propertyCode];

        }



        //public Tenant GetTenantById(Guid guid)
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            var property = this.GetPropertyByCode(propertyCode);
            return property.Tenants;
        }
        public void AddTenant(Tenant tenant)
        {
            var property = this.GetPropertyByCode(tenant.Property.Code);
            if (property.GetTenantByUnitNumber(tenant.UnitNumber) != null)
                Tenant.DuplicateException(tenant.UnitNumber, property.Code);
            property.AddTenant(tenant);
        }

        public Tenant GetTenantByUnitNumber(string unitNumber, string propCode)
        {
            var property = this.GetPropertyByCode(propCode);
            var tenant =  property.GetTenantByUnitNumber(unitNumber);
            if (tenant == null)
                Tenant.NotFoundException(unitNumber, propCode);
            return tenant;
        }

 

        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            foreach (var kvp in _properties)
            {
                foreach (var tenant in kvp.Value.Tenants)
                {
                    if (tenant.Id == tenantId)

                        return tenant.ActiveRequests;
                }
            }

            return null;

        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            var property = this.GetPropertyByCode(propCode);

            var tenant = property?.GetTenantByUnitNumber(tenantUnit);
            return tenant?.GetRequestByCode(requestCode);
           
        }

       
    }
}
