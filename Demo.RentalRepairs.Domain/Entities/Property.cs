﻿using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Property : Entity 
    {
        readonly DomainValidationService _validationService = new DomainValidationService();

        public string Name { get;   private set; }
        public string Code { get; private set; }
        public PropertyAddress Address { get; private set;  }
            public string PhoneNumber { get; private set;  }
        public PersonContactInfo Superintendent { get; private set;  }
        public List<string> Units { get; private set; }
        //-------------
        public string LoginEmail { get; set;  }
        public string NoReplyEmailAddress { get;  set; }

        public Property(PropertyInfo propertyInfo, DateTime? dateCreated = null, Guid? id = null) : base(dateCreated, id)
        {
            _validationService.ValidatePropertyInfo(propertyInfo);
            Name = propertyInfo.Name;
            Code = propertyInfo.Code;
            PhoneNumber = propertyInfo.PhoneNumber;
            Address = propertyInfo.Address;
            Superintendent = propertyInfo.Superintendent ;
            Units = propertyInfo.Units ;
            NoReplyEmailAddress = propertyInfo.NoReplyEmailAddress;

        }
        public Tenant RegisterTenant(PersonContactInfo contactInfo, string unitNumber)
        {
            var tenant = new Tenant(this, contactInfo, unitNumber);
            _validationService.ValidateTenant(tenant);

            return tenant;

        }
        
        public static void NotFoundException(string propertyCode)
        {
            throw new DomainEntityNotFoundException( "property_not_found", $"Property not found: {propertyCode }");
        }

        public static void DuplicateException(string propertyCode)
        {
            throw new DomainException("duplicate_property", $"Property already exist [{propertyCode}]");
        }


    }
}
