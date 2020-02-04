using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF
{
    public class PropertyRepositoryEntityFramework : IPropertyRepository
    {
        private readonly PropertiesContext _context;

        public PropertyRepositoryEntityFramework(PropertiesContext context)
        {
            _context = context;
        }
        public IEnumerable<Property> GetAllProperties()
        {
            return _context.PropertyTbl.Select(x=> new Property(x.Name , x.ID, x.PhoneNumber , x.Address , x.Superintendent , JsonConvert.DeserializeObject<List<string>>(x.Units ),x.DateCreated , x.IdCreated  )  ) ;
        }

        public void AddProperty(Property prop)
        {

            if (prop == null)
                throw new ArgumentNullException(nameof(prop));

            //if (_properties.ContainsKey(prop.Code))
            //    Property.DuplicateException(prop.Code);

            var propTbl = new PropertyTbl()
            {
                Address = prop.Address, ID = prop.Code,  Name = prop.Name,
                Units = JsonConvert.SerializeObject(prop.Units),
                Superintendent = prop.Superintendent, NoReplyEmailAddress = prop.NoReplyEmailAddress, DateCreated = prop.DateCreated , IdCreated = prop.Id 
            };
            _context.PropertyTbl.Add(propTbl);
            _context.SaveChanges();

        }

        public Property GetPropertyByCode(string propertyCode)
        {
            var p = _context.PropertyTbl.FirstOrDefault(x => x.ID == propertyCode);

            if (p == null) return null;

            var prop = new Property(p.Name, p.ID, p.PhoneNumber, p.Address,
                p.Superintendent, JsonConvert.DeserializeObject<List<string>>(p.Units), p.DateCreated, p.IdCreated)
            {
                NoReplyEmailAddress = p.NoReplyEmailAddress
            };
            return prop;

        }

        public void AddTenant(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            TenantTbl tenantTbl = new TenantTbl() {
                PropertyTblID = tenant.PropertyCode , ContactInfo = tenant.ContactInfo , ID = tenant.Id, UnitNumber = tenant.UnitNumber  
            };
             _context.TenantTbl.Add(tenantTbl);
            _context.SaveChanges();


        }

        public Tenant GetTenantByUnitNumber(string tenantCode, string propCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            throw new NotImplementedException();
        }

        public void AddTenantRequest(TenantRequest tTenantRequest)
        {
            throw new NotImplementedException();
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            throw new NotImplementedException();
        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {
            throw new NotImplementedException();
        }
    }
}
