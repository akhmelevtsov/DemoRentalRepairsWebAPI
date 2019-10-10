using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Tenant : Entity 
    {
        public PersonContactInfo ContactInfo { get; set;  }
        public Guid PropertyId { get; set;  }
        public Property Property { get; set;  }
        public string UnitNumber { get; private set;  }

        public List<TenantRequest> ActiveRequests => _activeRequests;

        private readonly List<TenantRequest> _activeRequests = new List<TenantRequest>();
        private List<TenantRequest> _closedRequests = new List<TenantRequest>();

        internal  Tenant(Property property, PersonContactInfo contactInfo, string unitNumber)
        {
            Property = property;
            ContactInfo = contactInfo;
            UnitNumber = unitNumber;
        }

        internal Tenant(string unitNumber)
        {
            UnitNumber = unitNumber;
        }
        public TenantRequest AddRequest(TenantRequestDoc tenantRequestDoc)
        {
            var tTenantRequest = new TenantRequest() {Tenant = this, Code = (_activeRequests.Count + 1).ToString( )};
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
