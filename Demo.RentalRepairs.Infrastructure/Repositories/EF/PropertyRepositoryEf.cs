using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF
{
    public class PropertyRepositoryEf : IPropertyRepository
    {
        private readonly PropertiesContext _context;
        private readonly EntityMapper _entityMapper = new EntityMapper();

        public PropertyRepositoryEf(PropertiesContext context)
        {
            _context = context;
        }
        public void AddWorker(Worker worker)
        {
            _context.WorkerTbl .Add(_entityMapper.CopyFrom(worker));
            _context.SaveChanges();
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            return _context.WorkerTbl .Select(x => _entityMapper.CopyFrom(x));
        }

        public Worker GetWorkerByEmail(string email)
        {
            return _context.WorkerTbl.Where(x => x.ContactInfo.EmailAddress == email).Select( x => _entityMapper.CopyFrom(x)).FirstOrDefault();
        }

        public IEnumerable<Property> GetAllProperties()
        {
            return _context.PropertyTbl.Select(x => _entityMapper.CopyFrom( x));
        }

        public void AddProperty(Property prop)
        {
            _context.PropertyTbl.Add(_entityMapper.CopyFrom(prop));
            _context.SaveChanges();
        }
        public Property GetPropertyByCode(string propertyCode)
        {
            return  _context.PropertyTbl.Where(x => x.ID == propertyCode).Select( y => _entityMapper.CopyFrom(y)).FirstOrDefault( );
        }
       
        public void AddTenant(Tenant tenant)
        {
            _context.TenantTbl.Add(_entityMapper.CopyFrom(tenant));
            _context.SaveChanges();
        }

        public Tenant GetTenantByUnitNumber(string unitNumber, string propCode)
        {
           return _context.TenantTbl
               .Where( ten => ten.PropertyTblID == propCode && ten.UnitNumber == unitNumber)
               .Include(p => p.Property )
               .Include( p=> p.Requests )
               .Select( y => _entityMapper.CopyFrom(y)).FirstOrDefault();
       
        }

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
           return  _context.TenantTbl
               .Where(t => t.PropertyTblID == propertyCode)
               .Include(p => p.Property)
               .Select(t => _entityMapper.CopyFrom(t)).ToList();
        }
        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            return _context.TenantRequestTbl
                .Where(t => t.Tenant.Property.ID == propertyCode)
                .Include(x => x.Tenant.Property )
              
                .Select(x => _entityMapper.CopyFrom(x));
        }

        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            return _context.TenantRequestTbl
                .Where(x => x.RequestChanges.Contains( workerEmail ))
                .Include(x => x.Tenant.Property )
                .Select(x => _entityMapper.CopyFrom(x)).ToList();
        }

     

        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            return _context.TenantRequestTbl
                .Where(x => x.TenantID  == tenantId)
                .Include( x => x.Tenant )
                .OrderBy(x => x.Code )
                .Select(x => _entityMapper.CopyFrom(x)) .ToList();
        }

        public Property FindPropertyByLoginEmail(string emailAddress)
        {
            return _context.PropertyTbl.Where(x => x.LoginEmail  == emailAddress)
                .Select(t => _entityMapper.CopyFrom(t)).FirstOrDefault();

        }


        public void AddTenantRequest(TenantRequest req)
        {
            _context.TenantRequestTbl.Add(_entityMapper.CopyFrom(req));
            _context.SaveChanges();
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            return _context.TenantRequestTbl
               
                .Where( x =>
                x.Tenant.Property.ID == propCode && x.Tenant.UnitNumber == tenantUnit && x.Code == requestCode)
                .Include(p => p.Tenant)
                .Include(p =>p.Tenant.Property )
                .Select(y => _entityMapper.CopyFrom(y)).FirstOrDefault();
        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            return _context.TenantRequestTbl
                .Where(x => x.ID == tenantRequestId)
                .Include(p => p.Tenant)
                .Select(y=> _entityMapper.CopyFrom(y)).FirstOrDefault() ;
        }
        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {

            var exist = _context.TenantRequestTbl.Find(tTenantRequest.Id);
            _context.Entry(exist).CurrentValues.SetValues(_entityMapper.CopyFrom(tTenantRequest));

 
            _context.SaveChanges();
        }

        public Tenant FindTenantByLoginEmail(string emailAddress)
        {
            return _context.TenantTbl.Where(x => x.LoginEmail  == emailAddress).Include( x => x.Property )
                .Select(x => _entityMapper.CopyFrom(x)).FirstOrDefault();
        }

        //public Worker FindWorkerByLoginEmail(string emailAddress)
        //{
        //    var req = _context.TenantRequestTbl.FirstOrDefault(x => x.WorkerEmail == emailAddress && x.ServiceWorkOrder != null  );


        //    return req == null
        //        ? null
        //        : new Worker(JsonConvert.DeserializeObject<ScheduleServiceWorkCommand>(req.ServiceWorkOrder).Person);
        //    //{  PersonContactInfo = JsonConvert.DeserializeObject<ServiceWorkOrder>(req.ServiceWorkOrder).Person};

        //}

      
    }
}
