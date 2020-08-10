using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Shared ;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb
{
    public class PropertyMongoDbRepository : IPropertyRepository
    {
        private readonly ModelMapper _modelMapper = new ModelMapper();
        private readonly IMongoCollection<PropertyModel> _properties;
        private readonly IMongoCollection<WorkerModel> _workers;


        public PropertyMongoDbRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("RentalRepairs");

            _properties = database.GetCollection<PropertyModel>("Properties");
            _workers = database.GetCollection<WorkerModel>("Workers");

        }
        public IEnumerable<Property> GetAllProperties()
        {
            return _properties.Find(property => true).ToList().Select(p => _modelMapper.CopyFrom(p));
        }

        public void AddProperty(Property prop)
        {
            _properties.InsertOne(_modelMapper.CopyFrom(prop));
        }
        public Property GetPropertyByCode(string propertyCode)
        {
            var p = _properties.Find<PropertyModel>(prop => prop.Id == propertyCode).FirstOrDefault();
            return p == null ? null : _modelMapper.CopyFrom(p);
        }

        public void AddTenant(Tenant tenant)
        {
            //var p = _properties.Find<PropertyModel>(prop => prop.Id == tenant.PropertyCode ).FirstOrDefault();
            //if (p==null)
            //    throw new DomainEntityNotFoundException("property_not_found", "property not found");
            //if (p.Tenants == null)
            //    p.Tenants = new List<TenantModel>();

            //p.Tenants.Add(_modelMapper.CopyFrom(tenant));
            //_properties.UpdateOne()
            //throw new NotImplementedException();

            //var filter = Builders<PropertyModel >.Filter.And(
            //    Builders<PropertyModel>.Filter.Where(x => x.Id == tenant.PropertyCode ),
            //    Builders<PropertyModel>.Filter.ElemMatch(x => x.Tenants , c => c.UnitNumber == tenant.UnitNumber ));
            //var update = Builders<PropertyModel>.Update.Push(x => x.Tenants[-1].Requests , newSubCategory);
            //_properties.FindOneAndUpdate(filter, update);



            var filter = Builders<PropertyModel>.Filter.Eq(p => p.Id, tenant.PropertyCode);
            var update = Builders<PropertyModel>.Update.Push(x => x.Tenants, _modelMapper.CopyFrom(tenant));
            _properties.FindOneAndUpdate(filter, update);

        }

        public void AddTenantRequest(TenantRequest tTenantRequest)
        {
            var filter = Builders<PropertyModel>.Filter.And(
                Builders<PropertyModel>.Filter.Where(x => x.Id == tTenantRequest.Tenant.PropertyCode),
                Builders<PropertyModel>.Filter.ElemMatch(x => x.Tenants, c => c.UnitNumber == tTenantRequest.Tenant.UnitNumber));
            var update = Builders<PropertyModel>.Update.Push(x => x.Tenants[-1].Requests, _modelMapper.CopyFrom(tTenantRequest));
            _properties.FindOneAndUpdate(filter, update);
        }

        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {
            //var filter = Builders<PropertyModel>.Filter.And(
            //    Builders<PropertyModel>.Filter.Where(x => x.Id == tTenantRequest.Tenant.PropertyCode),
            //    Builders<PropertyModel>.Filter.ElemMatch(x => x.Tenants, c => c.UnitNumber == tTenantRequest.Tenant.UnitNumber));
            //var update = Builders<PropertyModel>.Update
            //    //.Set(x => x.Tenants[-1].Requests[-1].RequestStatus, tTenantRequest.RequestStatus )
            //        .Set("Tenants.$[t].Requests.$[r].RequestStatus", tTenantRequest.RequestStatus)
            //    //.Set(x => x.Tenants[-1].Requests[-1].RequestChanges, _modelMapper .SerializeEvents( tTenantRequest.RequestChanges ))
            //    //.Set(x => x.Tenants[-1].Requests[-1].ServiceWorkOrderCount, tTenantRequest.ServiceWorkOrderCount)
            //    ;
            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("t.UnitNumber", tTenantRequest.Tenant.UnitNumber )),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("r.Code",  tTenantRequest.Code))
            };
            var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };


            //_properties.FindOneAndUpdate(filter, update );

            _properties.UpdateOne(x => x.Id == tTenantRequest.Tenant.PropertyCode,
                Builders<PropertyModel>.Update
                    .Set("Tenants.$[t].Requests.$[r].RequestStatus", tTenantRequest.RequestStatus)
                   .Set("Tenants.$[t].Requests.$[r].RequestChanges", _modelMapper .SerializeEvents( tTenantRequest.RequestChanges ))
                   .Set("Tenants.$[t].Requests.$[r].ServiceWorkOrderCount", tTenantRequest.ServiceWorkOrderCount)
                , updateOptions
            );
        }

        public void AddWorker(Worker worker)
        {
            _workers.InsertOne(_modelMapper.CopyFrom(worker));
        }

        public Tenant GetTenantByUnitNumber(string tenantCode, string propCode)
        {
            var p = _properties.Find<PropertyModel>(prop => prop.Id == propCode).FirstOrDefault();
            if (p?.Tenants == null || !p.Tenants.Any())
                return null;

            var tenantModel = p.Tenants.FirstOrDefault(x => x.UnitNumber == tenantCode);
            if (tenantModel == null)
                return null;
            tenantModel.PropertyModel = p;
            return _modelMapper.CopyFrom(tenantModel);
        }

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            var propertyModel = _properties.Find(p => p.Id == propertyCode).ToList().FirstOrDefault();
            return propertyModel == null ? new List<Tenant>() : propertyModel.Tenants.Select(x => _modelMapper.CopyFrom(x));
        }

    

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            var propertyModel = _properties.Find(p => p.Id == propCode).ToList().FirstOrDefault();
            var tenantModel = propertyModel?.Tenants.FirstOrDefault(x => x.UnitNumber == tenantUnit);
            if (tenantModel == null) return null;

            tenantModel.PropertyModel = propertyModel;
            var requestModel = tenantModel.Requests.FirstOrDefault(x => x.Code == requestCode);
            if (requestModel == null) return null;


            requestModel.TenantModel = tenantModel;
            return _modelMapper.CopyFrom(requestModel);


        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            return _workers.Find(worker => true).ToList().Select(p => _modelMapper.CopyFrom(p));
        }

        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            var tr = _properties.AsQueryable()
                .Where( p => p.Id == propertyCode)
                .SelectMany(x => x.Tenants)
                .SelectMany(y => y.Requests)
                .ToList();  //;.Select(z => _modelMapper.CopyFrom(z)).ToList();

            return MapTenantRequests(tr);
        }
        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            var tr = _properties.AsQueryable()
                .SelectMany(x => x.Tenants)
                .SelectMany(y => y.Requests)
                .Where(y => y.TenantID == tenantId )
                .ToList();  //;.Select(z => _modelMapper.CopyFrom(z)).ToList();

            return MapTenantRequests(tr);

        }
        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            //IEnumerable<TenantRequestModel>
            var tr = _properties.AsQueryable()
                .SelectMany(x =>  x.Tenants)
                .SelectMany(y => y.Requests)
                .Where(y => y.RequestChanges.Contains(workerEmail))
                .ToList();  //;.Select(z => _modelMapper.CopyFrom(z)).ToList();

            var list = MapTenantRequests(tr);
            return list;

        }

        private List<TenantRequest> MapTenantRequests(List<TenantRequestModel> tr)
        {
            var list = new List<TenantRequest>();
            foreach (var r in tr)
            {
                r.TenantModel = GetTenantModelById(r.TenantID);
                r.TenantModel.PropertyModel = GetPropertyModelById(r.TenantModel.PropertyTblID);

                var tenantReq = _modelMapper.CopyFrom(r);
                list.Add(tenantReq);
            }

            return list;
        }

        public Worker GetWorkerByEmail(string email)
        {
            return _workers.Find(x => x.ContactInfo.EmailAddress == email).ToList().Select(p => _modelMapper.CopyFrom(p)).FirstOrDefault( );
        }

        private TenantModel GetTenantModelById(Guid tenantModelId)
        {
            return _properties.AsQueryable()
                .SelectMany(x => x.Tenants)
                .FirstOrDefault(x => x.Id == tenantModelId);

        }
        private PropertyModel GetPropertyModelById(string modelId)
        {
            return _properties.AsQueryable()
                .FirstOrDefault(x => x.Id == modelId);

        }
    }
}
