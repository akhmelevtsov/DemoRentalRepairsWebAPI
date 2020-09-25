using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi
{
    internal class EntityMapper
    {
        //-----------------------------------------------------------
        internal PropertyTbl CopyFrom(Property prop)
        {
            var propTbl = new PropertyTbl(prop.Code )
            {
                Address = JsonConvert.SerializeObject(prop.Address ),
                Name = prop.Name,
                PhoneNumber = prop.PhoneNumber,
                Units = JsonConvert.SerializeObject(prop.Units),
                Superintendent = JsonConvert.SerializeObject(prop.Superintendent),
                NoReplyEmailAddress = prop.NoReplyEmailAddress,
                DateCreated = prop.DateCreated,
                IdCreated = prop.Id
            };
            return propTbl;
        }
        internal Property CopyFrom(PropertyTbl p)
        {
            var prop = new Property(new RegisterPropertyCommand(p.Name,
                                                                p.ID, 
                                                                JsonConvert.DeserializeObject< PropertyAddress>(p.Address), 
                                                                p.PhoneNumber,
                                                                 JsonConvert.DeserializeObject< PersonContactInfo>( p.Superintendent),
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
            WorkerTbl tenantTbl = new WorkerTbl(worker.PersonContactInfo.EmailAddress )
            {
                ContactInfo = JsonConvert.SerializeObject(worker.PersonContactInfo) ,
                ID = worker.Id,
                DateCreated = worker.DateCreated 
                
            };
            return tenantTbl;
        }
        internal Worker CopyFrom(WorkerTbl workerTbl)
        {
            return new Worker(
                JsonConvert.DeserializeObject< PersonContactInfo > (workerTbl.ContactInfo),
                workerTbl.DateCreated,
                workerTbl.ID)
            {
              
            };
        }

        //----
        internal TenantTbl CopyFrom(Tenant tenant)
        {
            TenantTbl tenantTbl = new TenantTbl(tenant.PropertyCode ,  tenant.UnitNumber  )
            {
              
                ContactInfo = JsonConvert.SerializeObject(tenant.ContactInfo),
                ID = tenant.Id,
                DateCreated = tenant.DateCreated 
            };
            return tenantTbl;
        }
        internal Tenant CopyFrom(TenantTbl tenantTbl, int requestCount=0, PropertyTbl propertyTbl= null)
        {
            return new Tenant(
                CopyFrom(propertyTbl),
                JsonConvert.DeserializeObject <PersonContactInfo> (tenantTbl.ContactInfo),
                tenantTbl.UnitNumber,
                tenantTbl.DateCreated,
                tenantTbl.ID)
            {
                RequestsNum = requestCount //   tenantTbl.Requests?.Count() ?? 0,
              
            };
        }

        //----- Requests
        internal TenantRequest CopyFrom(TenantRequestTbl r, TenantTbl tenantTbl, int requestCount, PropertyTbl propertyTbl)
        {
            TenantRequestStatusEnum reqStatus = (TenantRequestStatusEnum)Enum.Parse(typeof(TenantRequestStatusEnum), r.RequestStatus);
            var req =
                new TenantRequest(
                    CopyFrom(tenantTbl, requestCount, propertyTbl ), r.Code,reqStatus, r.DateCreated, r.ID)
                {
                   
                    RequestChanges = r.RequestChanges == null ? null : DeserializeObject(r.RequestChanges ),

                    ServiceWorkOrderCount = r.ServiceWorkOrderCount,
                   
                };

            return req;
        }
        internal TenantRequest CopyFrom(TenantRequestTbl r, Tenant tenant)
        {
            TenantRequestStatusEnum reqStatus = (TenantRequestStatusEnum)Enum.Parse(typeof(TenantRequestStatusEnum), r.RequestStatus);

            var req =
                new TenantRequest(
                    tenant, r.Code, reqStatus, r.DateCreated, r.ID)
                {

                    RequestChanges = r.RequestChanges == null ? null : DeserializeObject(r.RequestChanges),

                    ServiceWorkOrderCount = r.ServiceWorkOrderCount,

                };

            return req;
        }


        internal TenantRequestTbl CopyFrom(TenantRequest req)
        {
            
            return new TenantRequestTbl(req.TenantEmail , req.Code )
            {
              
                DateCreated = req.DateCreated,
                ID = req.Id,
                TenantID = req.Tenant.Id,
                PropertyCode = req.Tenant.Property.Code ,
                UnitNumber = req.Tenant.UnitNumber , 
                RequestStatus = req.RequestStatus.ToString( ),
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
