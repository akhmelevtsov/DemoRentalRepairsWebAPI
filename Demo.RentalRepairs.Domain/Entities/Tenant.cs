using System;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Framework;
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

        public int RequestsNum { get; set; }

        public string LoginEmail { get; set;  }

        public  Tenant(Property property, PersonContactInfo contactInfo, string unitNumber, DateTime? dateCreated= null, Guid? id=null) : base(dateCreated, id)
        {
            Property = property;
            PropertyCode = property.Code;
            ContactInfo = contactInfo;
            UnitNumber = unitNumber;
        }
        
        public TenantRequest AddRequest(TenantRequestDoc tenantRequestDoc)
        {
            var tTenantRequest = new TenantRequest(this, (RequestsNum  + 1).ToString());
            tTenantRequest.ChangeStatus( TenantRequestStatusEnum.RequestReceived ,tenantRequestDoc);

            return tTenantRequest;

        }

        //public static void NotFoundException(string propertyUnit, string propertyCode)
        //{
        //    throw new DomainEntityNotFoundException("tenant_not_found", $"Property : {propertyCode} - tenant not found by unit number: {propertyUnit}");
        //}
    }
}
