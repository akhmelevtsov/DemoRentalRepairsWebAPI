using System;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Tenant : Entity
    {
        readonly DomainValidationService _validationService = new DomainValidationService();
        public PersonContactInfo ContactInfo { get; set;  }
        public string PropertyCode { get; set;  }
        public string UnitNumber { get; private set;  }
        public Property Property { get; set;  }

        public int RequestsNum { get; set; }

        

        public  Tenant(Property property, PersonContactInfo contactInfo, string unitNumber, DateTime? dateCreated= null, Guid? id=null) : base(dateCreated, id)
        {
            Property = property;
            PropertyCode = property.Code;
            ContactInfo = contactInfo;
            UnitNumber = unitNumber;
        }
        
        public TenantRequest AddRequest(RegisterTenantRequestCommand registerTenantRequestCommand)
        {
            _validationService.Validate(registerTenantRequestCommand);
            RequestsNum++;
            var tTenantRequest = new TenantRequest(this, (RequestsNum).ToString());
            tTenantRequest.ExecuteCommand( registerTenantRequestCommand);

            return tTenantRequest;

        }

        //public static void NotFoundException(string propertyUnit, string propertyCode)
        //{
        //    throw new DomainEntityNotFoundException("tenant_not_found", $"Property : {propertyCode} - tenant not found by unit number: {propertyUnit}");
        //}
    }
}
