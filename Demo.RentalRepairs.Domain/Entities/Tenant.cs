using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Tenant : Entity, ITenantFields
    {
        public PersonContactInfo ContactInfo { get; set;  }
        public string PropertyCode { get; set;  }
        public string UnitNumber { get; private set;  }
        public Property Property { get; set;  }

        public List<TenantRequest> ActiveRequests => _activeRequests;
        public int RequestsNum { get; set; }

        private readonly List<TenantRequest> _activeRequests = new List<TenantRequest>();
        private readonly List<TenantRequest> _closedRequests = new List<TenantRequest>();

        internal  Tenant(Property property, PersonContactInfo contactInfo, string unitNumber) : base(PropertyDomainService.DateTimeProvider)
        {
            Property = property;
            PropertyCode = property.Code;
            ContactInfo = contactInfo;
            UnitNumber = unitNumber;
        }
        public Tenant(Property property, PersonContactInfo contactInfo, string unitNumber, DateTime dateCreated, Guid idGuid) : 
            this(property , contactInfo , unitNumber)
        {
            base.UpdateCreateInfo(dateCreated, idGuid);
        }
        internal Tenant(string unitNumber)
        {
            UnitNumber = unitNumber;
        }
        public TenantRequest AddRequest(TenantRequestDoc tenantRequestDoc)
        {
            //var tTenantRequest = new TenantRequest(this,  (_activeRequests.Count + 1).ToString());
            var tTenantRequest = new TenantRequest(this, (RequestsNum  + 1).ToString());
            tTenantRequest.ChangeStatus( TenantRequestStatusEnum.RequestReceived ,tenantRequestDoc);

            _activeRequests.Add(tTenantRequest);
            return tTenantRequest;

        }

       
        public TenantRequest GetActiveRequestById(Guid  id)
        {
             return   _activeRequests.FirstOrDefault(x => x.Id == id);
        }


        public TenantRequest GetRequestByCode(string requestCode)
        {
            return _activeRequests.FirstOrDefault(x => x.Code == requestCode);

        }
        public static void DuplicateException(string unitNumber, string propertyCode)
        {
            throw new DomainException("duplicate_tenant", $"Other tenant is already registered in unit [{unitNumber}]");
        }

        public static void NotFoundException(string propertyUnit, string propertyCode)
        {
            throw new DomainEntityNotFoundException("tenant_not_found", $"Property : {propertyCode} - tenant not found by unit number: {propertyUnit}");
        }
    }
}
