using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation.Results;

namespace Demo.RentalRepairs.Domain.Entities.Extensions
{
    public static class DomainValidations
    {
        public static void Validate(this Property property)
        {
            var  validator = new PropertyValidator();
 
            var results = validator.Validate(property);
            if (!results.IsValid)
            {
                throw new DomainValidationException("create_property_validation",  results.Errors);
            }
        }
        public static void ValidatePropertyCode(string propertyCode)
        {
           
            var validator = new PropertyCodeValidator();
            ValidationResult results = validator.Validate(propertyCode);
            if (!results.IsValid)
            {
                throw new DomainValidationException("property_code_validation", results.Errors);
            }

        }

        public static void Validate(this Tenant tenant)
        {
            var validator = new TenantValidator();
            var results = validator.Validate(tenant);
            if (!results.IsValid)
            {
                throw new DomainValidationException("add_tenant_validation", results.Errors);
            }
        }

        public static void ValidatePropertyUnit(string propertyUnit)
        {
            var validator = new UnitNumberValidator() ;
            var results = validator.Validate(propertyUnit);
            if (!results.IsValid)
            {
                throw new DomainValidationException("property_unit_validation", results.Errors);
            }
        }

        public static void ValidatePersonContactInfo(PersonContactInfo personContactInfo)
        {
            var validator = new PersonContactInfoValidator();
            var results = validator.Validate(personContactInfo);
            if (!results.IsValid)
            {
                throw new DomainValidationException("personContactInfo_validation", results.Errors);
            }
        }


    }
}
