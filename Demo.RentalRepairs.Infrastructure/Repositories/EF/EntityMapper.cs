using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
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
                IdCreated = prop.Id
            };
            return propTbl;
        }
        internal Property CopyFrom(PropertyTbl p)
        {
            var prop = new Property(new RegisterPropertyCommand(p.Name, p.ID,  p.Address, p.PhoneNumber,
                p.Superintendent,
                JsonConvert.DeserializeObject<List<string>>(p.Units),
                p.NoReplyEmailAddress),
                p.DateCreated, 
                p.IdCreated)
            {
            };
            return prop;
        }
        //----
        internal WorkerTbl CopyFrom(Worker worker)
        {
            WorkerTbl tenantTbl = new WorkerTbl()
            {
               
                ContactInfo = worker.PersonContactInfo ,
                ID = worker.Id
                
            };
            return tenantTbl;
        }
        internal Worker CopyFrom(WorkerTbl workerTbl)
        {
            return new Worker(

                workerTbl.ContactInfo,
                workerTbl.DateCreated,
                workerTbl.ID)
            {
              
            };
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
              
            };
        }

        //----- Requests
        internal TenantRequest CopyFrom(TenantRequestTbl r)
        {
            var req =
                new TenantRequest(
                    CopyFrom(r.Tenant), r.Code, r.RequestStatus, r.DateCreated, r.ID)
                {
                    //RequestCommand = r.RequestDoc == null ? null : JsonConvert.DeserializeObject<RegisterTenantRequestCommand>(r.RequestDoc),
                    //RejectNotesCommand = r.RejectNotes == null ? null : JsonConvert.DeserializeObject<RejectTenantRequestCommand>(r.RejectNotes),
                    //ScheduleWorkCommand = r.ServiceWorkOrder == null ? null : JsonConvert.DeserializeObject<ScheduleServiceWorkCommand>(r.ServiceWorkOrder),
                    RequestChanges = r.RequestChanges == null ? null : DeserializeObject(r.RequestChanges ),

                    ServiceWorkOrderCount = r.ServiceWorkOrderCount,
                   
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
                RequestChanges = req.RequestChanges == null? null: SerializeObject(req.RequestChanges ),
                ServiceWorkOrderCount = req.ServiceWorkOrderCount
              

            };
        }

        private string SerializeObject(List<TenantRequestChange> changes)
        {
            
            var r = JsonConvert.SerializeObject(changes,JsonSettings() );
            return r;
        }
        private List<TenantRequestChange> DeserializeObject(string changes)
        {
            var list =  JsonConvert.DeserializeObject<List<TenantRequestChange>>(changes,JsonSettings() );

            return list;
        }

        private  JsonSerializerSettings JsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto 
            };
            return settings;
        }
    }
}
