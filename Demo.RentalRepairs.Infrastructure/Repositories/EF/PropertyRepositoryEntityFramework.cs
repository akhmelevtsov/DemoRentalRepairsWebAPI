using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Microsoft.EntityFrameworkCore;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF
{
    public class PropertyRepositoryEntityFramework : IPropertyRepository
    {
        private readonly PropertiesContext _context;

        public PropertyRepositoryEntityFramework(PropertiesContext context)
        {
            _context = context;
            //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IEnumerable<Property> GetAllProperties()
        {
            //return _context.PropertyTbl.Select(x => new Property(x.Name, x.ID, x.PhoneNumber, x.Address, x.Superintendent, JsonConvert.DeserializeObject<List<string>>(x.Units), x.DateCreated, x.IdCreated));
            return _context.PropertyTbl.Select(x => CopyFrom( x));
        }

        public void AddProperty(Property prop)
        {
            _context.PropertyTbl.Add(CopyFrom(prop));
            _context.SaveChanges();
        }
        public Property GetPropertyByCode(string propertyCode)
        {
            return  _context.PropertyTbl.Where(x => x.ID == propertyCode).Select( y => CopyFrom(y)).FirstOrDefault( );
        }
       
        public void AddTenant(Tenant tenant)
        {
            _context.TenantTbl.Add(CopyFrom(tenant));
            _context.SaveChanges();
        }

        public Tenant GetTenantByUnitNumber(string unitNumber, string propCode)
        {
           return _context.TenantTbl.Where( ten => ten.PropertyTblID == propCode && ten.UnitNumber == unitNumber)
               .Select( y => CopyFrom(y)).FirstOrDefault();
        }

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
           return  _context.TenantTbl.Where(t => t.PropertyTblID == propertyCode).Select(t => CopyFrom(t)).ToList();
        }
        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            return _context.TenantRequestTbl.Where(x => x.ID == tenantId).Select(y => CopyFrom(y)).ToList();
        }


        public void AddTenantRequest(TenantRequest req)
        {
            _context.TenantRequestTbl.Add(CopyFrom(req));
            _context.SaveChanges();
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            return _context.TenantRequestTbl.Where( x =>
                x.Tenant.Property.ID == propCode && x.Tenant.UnitNumber == tenantUnit && x.Code == requestCode).Select(y => CopyFrom(y)).FirstOrDefault();
        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            return _context.TenantRequestTbl.Where(x => x.ID == tenantRequestId).Select(y=> CopyFrom(y)).FirstOrDefault() ;
        }
        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {

            var exist = _context.TenantRequestTbl.Find(tTenantRequest.Id);
            _context.Entry(exist).CurrentValues.SetValues(CopyFrom(tTenantRequest));

            ////_context.TenantRequestTbl.Update(CopyFrom(tTenantRequest));
            //var req = CopyFrom(tTenantRequest);
            //_context.Attach(req);
            //_context.Entry(req).Property("RequestStatus").IsModified = true;
            _context.SaveChanges();
        }
        //-----------------------------------------------------------
        private PropertyTbl CopyFrom(Property prop)
        {
            var propTbl = new PropertyTbl()
            {
                Address = prop.Address,
                ID = prop.Code,
                Name = prop.Name,
                Units = JsonConvert.SerializeObject(prop.Units),
                Superintendent = prop.Superintendent,
                NoReplyEmailAddress = prop.NoReplyEmailAddress,
                DateCreated = prop.DateCreated,
                IdCreated = prop.Id
            };
            return propTbl;
        }
        private Property CopyFrom(PropertyTbl p)
        {
            var prop = new Property(p.Name, p.ID, p.PhoneNumber, p.Address,
                p.Superintendent, JsonConvert.DeserializeObject<List<string>>(p.Units), p.DateCreated, p.IdCreated)
            {
                NoReplyEmailAddress = p.NoReplyEmailAddress
            };
            return prop;
        }
        //----
        private TenantTbl CopyFrom(Tenant tenant)
        {
            TenantTbl tenantTbl = new TenantTbl()
            {
                PropertyTblID = tenant.PropertyCode,
                ContactInfo = tenant.ContactInfo,
                ID = tenant.Id,
                UnitNumber = tenant.UnitNumber
            };
            return tenantTbl;
        }
        private Tenant CopyFrom(TenantTbl tenantTbl)
        {
            return new Tenant(
                CopyFrom(tenantTbl.Property),
                tenantTbl.ContactInfo,
                tenantTbl.UnitNumber,
                tenantTbl.DateCreated,
                tenantTbl.ID)
            {
                RequestsNum = tenantTbl.Requests?.Count () ?? 0
            };
        }

        //----- Requests
        private TenantRequest CopyFrom(TenantRequestTbl r)
        {
            var req =
                new TenantRequest(
                    CopyFrom(r.Tenant), r.Code, r.RequestStatus, r.DateCreated, r.ID)
                {
                    RequestDoc  = r.RequestDoc == null ? null : JsonConvert.DeserializeObject<TenantRequestDoc>(r.RequestDoc),
                    RejectNotes = r.RejectNotes == null ? null : JsonConvert.DeserializeObject<TenantRequestRejectNotes>(r.RejectNotes ),
                    ServiceWorkOrder = r.ServiceWorkOrder == null ? null : JsonConvert.DeserializeObject<ServiceWorkOrder >(r.ServiceWorkOrder),
                    ServiceWorkOrderCount = r.ServiceWorkOrderCount 
                };
           
            return req;
        }
        private TenantRequestTbl CopyFrom(TenantRequest req)
        {
            return new TenantRequestTbl()
            {
                Code = req.Code,
                DateCreated = req.DateCreated,
                ID = req.Id,
                TenantID = req.Tenant.Id,

                RequestStatus = req.RequestStatus,
                RequestDoc = req.RequestDoc == null ? null : JsonConvert.SerializeObject(req.RequestDoc),
                RejectNotes = req.RejectNotes == null ? null : JsonConvert.SerializeObject( req.RejectNotes ),
                ServiceWorkOrder = req.ServiceWorkOrder == null ? null : JsonConvert.SerializeObject(req.ServiceWorkOrder),
                ServiceWorkOrderCount =  req.ServiceWorkOrderCount

            };
        }
    }
}
