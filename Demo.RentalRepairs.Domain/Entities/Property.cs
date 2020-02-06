using System;
using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Demo.RentalRepairs.Domain.Entities
{
    public class Property : Entity, IPropertyFields
    {
        public string Name { get;   private set; }
        public string Code { get; private set; }
        public PropertyAddress Address { get; private set;  }
        public string PhoneNumber { get; private set;  }
        public PersonContactInfo Superintendent { get; private set;  }
        public List<string> Units { get; private set; }
        public string NoReplyEmailAddress { get;  set; }

        //public List<Tenant> Tenants { get; } = new List<Tenant>();


        public  Property(string name, string code, string phoneNumber, PropertyAddress propertyAddress,
            PersonContactInfo superintendent, List<string> units) : base(PropertyDomainService.DateTimeProvider)
        {
            Name = name;
            Code = code;
            PhoneNumber = phoneNumber;
            Address = propertyAddress;
            Superintendent = superintendent;
            Units = units;
        }

        public Property(string name, string code, string phoneNumber, PropertyAddress propertyAddress,
            PersonContactInfo superintendent, List<string> units, 
            DateTime dateCreated, Guid idGuid) : this( name, code,  phoneNumber,  propertyAddress,
             superintendent, units)  // create from repo
        {
            base.UpdateCreateInfo(dateCreated, idGuid);
        }

        internal Property(string propertyCode) // for validations
        {
            Code = propertyCode;
        }

        //public Tenant AddTenant(Tenant tenant )
        //{
        //    tenant.Property = this;
        //    Tenants.Add(tenant);
        //    return tenant;

        //}

       


        //public  Tenant GetTenantByUnitNumber(string unitNumber)
        //{
        //    return Tenants.FirstOrDefault(x => x.UnitNumber  == unitNumber);
        //}

        public static void NotFoundException(string propertyCode)
        {
            throw new DomainEntityNotFoundException( "property_not_found", $"Property not found: {propertyCode }");
        }

        public static void DuplicateException(string propertyCode)
        {
            throw new DomainException("duplicate_property", $"Property already exist [{propertyCode}]");
        }


        private void AssignProperties(string name, string code, string phoneNumber, PropertyAddress propertyAddress,
            PersonContactInfo superintendent, List<string> units)
        {
            Name = name;
            Code = code;
            PhoneNumber = phoneNumber;
            Address = propertyAddress;
            Superintendent = superintendent;
            Units = units;
        }
    }
}
