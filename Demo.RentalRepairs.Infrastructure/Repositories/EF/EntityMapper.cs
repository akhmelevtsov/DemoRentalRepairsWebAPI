using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF
{
    internal class EntityMapper
    {
        //-----------------------------------------------------------
        internal PropertyTbl CopyFrom(Property prop)
        {
            var propTbl = new PropertyTbl()
            {
                Address = prop.Address,
                ID = prop.Code,
                Name = prop.Name,
                PhoneNumber = prop.PhoneNumber, 
                Units = JsonConvert.SerializeObject(prop.Units),
                Superintendent = prop.Superintendent,
                NoReplyEmailAddress = prop.NoReplyEmailAddress,
                DateCreated = prop.DateCreated,
                LoginEmail = prop.LoginEmail ,
                IdCreated = prop.Id
            };
            return propTbl;
        }
        internal Property CopyFrom(PropertyTbl p)
        {
            var prop = new Property(new PropertyInfo(p.Name, p.ID,  p.Address, p.PhoneNumber,
                p.Superintendent, JsonConvert.DeserializeObject<List<string>>(p.Units), p.NoReplyEmailAddress), p.DateCreated, p.IdCreated)
            {
               
                LoginEmail = p.LoginEmail
            };
            return prop;
        }
        //----
        internal TenantTbl CopyFrom(Tenant tenant)
        {
            TenantTbl tenantTbl = new TenantTbl()
            {
                PropertyTblID = tenant.PropertyCode,
                ContactInfo = tenant.ContactInfo,
                ID = tenant.Id,
                UnitNumber = tenant.UnitNumber,
                LoginEmail = tenant.LoginEmail,
            };
            return tenantTbl;
        }
        internal Tenant CopyFrom(TenantTbl tenantTbl)
        {
            return new Tenant(
                CopyFrom(tenantTbl.Property),
                tenantTbl.ContactInfo,
                tenantTbl.UnitNumber,
                tenantTbl.DateCreated,
                tenantTbl.ID)
            {
                RequestsNum = tenantTbl.Requests?.Count() ?? 0,
                LoginEmail = tenantTbl.LoginEmail
            };
        }

        //----- Requests
        internal TenantRequest CopyFrom(TenantRequestTbl r)
        {
            var req =
                new TenantRequest(
                    CopyFrom(r.Tenant), r.Code, r.RequestStatus, r.DateCreated, r.ID)
                {
                    RequestDoc = r.RequestDoc == null ? null : JsonConvert.DeserializeObject<TenantRequestDoc>(r.RequestDoc),
                    RejectNotes = r.RejectNotes == null ? null : JsonConvert.DeserializeObject<TenantRequestRejectNotes>(r.RejectNotes),
                    ServiceWorkOrder = r.ServiceWorkOrder == null ? null : JsonConvert.DeserializeObject<ServiceWorkOrder>(r.ServiceWorkOrder),
                    ServiceWorkOrderCount = r.ServiceWorkOrderCount,
                    WorkerEmail = r.WorkerEmail 
                };

            return req;
        }
        internal TenantRequestTbl CopyFrom(TenantRequest req)
        {
            return new TenantRequestTbl()
            {
                Code = req.Code,
                DateCreated = req.DateCreated,
                ID = req.Id,
                TenantID = req.Tenant.Id,

                RequestStatus = req.RequestStatus,
                RequestDoc = req.RequestDoc == null ? null : JsonConvert.SerializeObject(req.RequestDoc),
                RejectNotes = req.RejectNotes == null ? null : JsonConvert.SerializeObject(req.RejectNotes),
                ServiceWorkOrder = req.ServiceWorkOrder == null ? null : JsonConvert.SerializeObject(req.ServiceWorkOrder),
                ServiceWorkOrderCount = req.ServiceWorkOrderCount,
                WorkerEmail = req.WorkerEmail 

            };
        }
    }
}
