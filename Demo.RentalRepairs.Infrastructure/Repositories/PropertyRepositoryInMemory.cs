﻿using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Exceptions;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories
{
    public class PropertyRepositoryInMemory : IPropertyRepository
    {
        private readonly Dictionary< string,Property> _properties = new Dictionary<string, Property>();

        private readonly List<Tenant> _tenants = new List<Tenant>();
        private readonly Dictionary<Guid,TenantRequest >  _requests = new Dictionary<Guid, TenantRequest>();

        private List< Worker> _workers = new List<Worker>();

       

        public void AddTenantRequest(TenantRequest tenantRequest)
        {
            _requests.Add(tenantRequest.Id, tenantRequest);
        }

        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {
            if (!_requests.ContainsKey(tTenantRequest.Id))
            
                throw new DomainEntityNotFoundException("tenant_request_not_found",
                    $"Tenant request not found by id: {tTenantRequest} ");
           
            _requests[tTenantRequest.Id] = tTenantRequest;
            
        }

        //public Tenant FindTenantByLoginEmail(string emailAddress)
        //{
        //    return _tenants.FirstOrDefault(x => x.LoginEmail  == emailAddress);
        //}

        //public Worker FindWorkerByLoginEmail(string emailAddress)
        //{
        //    var request = _requests.Values.FirstOrDefault(x =>
        //        (x.ScheduleWorkCommand != null && x.ScheduleWorkCommand?.Person?.EmailAddress == emailAddress));
        //    return request == null ? null : new Worker(request.ScheduleWorkCommand.Person);
        //}

        public void AddWorker(Worker worker)
        {
            _workers.Add(worker);
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            return _workers;
        }

        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            return _requests.Values.Where(x => x.PropertyId == propertyCode);
        }

        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            return _requests.Values.Where(x => JsonConvert.SerializeObject( x.RequestChanges).Contains(workerEmail));
        }

        public Worker GetWorkerByEmail(string email)
        {
            return _workers.FirstOrDefault(x => x.PersonContactInfo.EmailAddress == email);
        }

        public TenantRequest GetTenantRequestById(Guid tenantRequestId)
        {
            if (!_requests.ContainsKey(tenantRequestId))
                throw new DomainEntityNotFoundException("tenant_request_not_found",
                    $"Tenant request not found by id: {tenantRequestId} ");

            return _requests[tenantRequestId];
 
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
                return null;
            return _properties[propertyCode];

        }



        //public Tenant GetTenantById(Guid guid)
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            return _tenants.Where(x => x.PropertyCode == propertyCode);

        }
        public void AddTenant(Tenant tenant)
        {
            _tenants.Add(tenant);
 
        }

        public Tenant GetTenantByUnitNumber(string unitNumber, string propCode)
        {
            var tenant = _tenants.FirstOrDefault(x => x.UnitNumber == unitNumber && x.PropertyCode == propCode);
            if (tenant != null )
               tenant.RequestsNum = GetTenantRequests(tenant.Id).Count();
            return tenant;
        }

        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            return _requests.Values.Where(x => x.Tenant.Id == tenantId);

        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {

            var tenant = this.GetTenantByUnitNumber(tenantUnit, propCode);
            return   _requests.Values.FirstOrDefault(  x => x.Tenant.Id == tenant.Id && x.Code == requestCode);
 

        }

       
    }
}
