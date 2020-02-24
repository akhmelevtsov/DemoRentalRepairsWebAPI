﻿using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Services
{
    public class PropertyDomainService
    {
        public static  IDateTimeProvider DateTimeProvider { get; private set; }
        readonly ValidationService _validationService = new ValidationService();

       

        public PropertyDomainService(IDateTimeProvider dateTimeProvider = null)
        {

            DateTimeProvider = dateTimeProvider ?? new DateTimeProvider();
        }

        public Property CreateProperty(string name, string code, PropertyAddress propertyAddress, string phoneNumber,
            PersonContactInfo superintendentInfo, List<string> units)
        {
            var prop = new Property(name, code, phoneNumber, propertyAddress, superintendentInfo, units);
            _validationService.ValidateProperty(prop);
            
            return prop;
        }

        public Tenant AddTenant(Property property, PersonContactInfo contactInfo, string unitNumber)
        {
           
            var tenant = new Tenant(property, contactInfo, unitNumber);
            _validationService.ValidateTenant(tenant);
           
           
            return tenant;

        }

        public TenantRequest RegisterTenantRequest(Tenant tenant, TenantRequestDoc tenantRequestDoc)
        {
            if (tenant == null)
                throw new ArgumentException(nameof (tenant ));

            var tTenantRequest = tenant.AddRequest(tenantRequestDoc);
            return tTenantRequest;

        }

    
    }
}
