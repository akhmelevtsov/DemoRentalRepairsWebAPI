using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb
{
    internal class ModelMapper
    {
        internal PropertyModel CopyFrom(Property prop)
        {
            var propModel = new PropertyModel()
            {
                Address = prop.Address,
                Id = prop.Code,
                Name = prop.Name,
                PhoneNumber = prop.PhoneNumber,
                Units =  new List<string>( prop.Units ),
                Superintendent = prop.Superintendent,
                NoReplyEmailAddress = prop.NoReplyEmailAddress,
                DateCreated = prop.DateCreated,
                IdCreated = prop.Id
            };
            return propModel;
        }
        internal Property CopyFrom(PropertyModel p)
        {
            var prop = new Property(new RegisterPropertyCommand(p.Name, p.Id , p.Address, p.PhoneNumber,
                    p.Superintendent,
                    p.Units,
                    p.NoReplyEmailAddress),
                p.DateCreated,
                p.IdCreated)
            {
            };
            return prop;
        }
        //----
        internal WorkerModel CopyFrom(Worker worker)
        {
            WorkerModel workerModel = new WorkerModel()
            {

                ContactInfo = worker.PersonContactInfo,
                Id = worker.Id

            };
            return workerModel;
        }
        internal Worker CopyFrom(WorkerModel  workerModel )
        {
            return new Worker(

                workerModel.ContactInfo,
                workerModel.DateCreated,
                workerModel.Id )
            {

            };
        }
        //----
     
        internal TenantModel CopyFrom(Tenant tenant)
        {
            var tenantModel = new TenantModel()
            {
                PropertyTblID = tenant.PropertyCode,
                ContactInfo = tenant.ContactInfo,
                Id = tenant.Id,
                UnitNumber = tenant.UnitNumber,

            };
            return tenantModel;
        }
        internal Tenant CopyFrom(TenantModel tenantModel)
        {

            return new Tenant(
                CopyFrom(tenantModel.PropertyModel),
                tenantModel.ContactInfo,
                tenantModel.UnitNumber,
                tenantModel.DateCreated,
                tenantModel.Id)
            {
                RequestsNum = tenantModel.Requests?.Count() ?? 0,

            };
        }


        internal TenantRequest CopyFrom(TenantRequestModel r)
        {
            var req =
                new TenantRequest(
                    CopyFrom(r.TenantModel), r.Code, r.RequestStatus, r.DateCreated, r.ID)
                {
  
                    RequestChanges = DeserializeEvents(r.RequestChanges ),

                    ServiceWorkOrderCount = r.ServiceWorkOrderCount,

                };

            return req;
        }
      

        internal TenantRequestModel  CopyFrom(TenantRequest req)
        {
            return new TenantRequestModel()
            {
                Code = req.Code,
                DateCreated = req.DateCreated,
                ID = req.Id,
                TenantID = req.Tenant.Id,

                RequestStatus = req.RequestStatus,
                RequestChanges = SerializeEvents(req.RequestChanges ), //   req.RequestChanges == null ? null : SerializeObject(req.RequestChanges),

                ServiceWorkOrderCount = req.ServiceWorkOrderCount


            };
        }
        public  string SerializeEvents(List<TenantRequestChange> requestChanges)
        {
            return requestChanges == null ? null : SerializeObject(requestChanges);
        }

        public  List<TenantRequestChange> DeserializeEvents(string requestChanges)
        {
            return requestChanges == null ? null : DeserializeObject(requestChanges);
        }
        //internal TenantRequestEventModel  CopyFrom(TenantRequestChange x)
        //{
        //    return new TenantRequestEventModel() { Num = x.Num , TenantRequestStatus = x.TenantRequestStatus , Command = }
        //}

        private string SerializeObject(List<TenantRequestChange> changes)
        {

            var r = JsonConvert.SerializeObject(changes, JsonSettings());
            return r;
        }
        private List<TenantRequestChange> DeserializeObject(string changes)
        {
            var list = JsonConvert.DeserializeObject<List<TenantRequestChange>>(changes, JsonSettings());

            return list;
        }

        private JsonSerializerSettings JsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            return settings;
        }
    }
}
