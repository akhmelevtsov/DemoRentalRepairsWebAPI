using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation.Results;

namespace Demo.RentalRepairs.Domain.Services
{
    public class ValidationRulesService
    {
        public  ValidationResult ValidateProperty( IProperty property)
        {
            var validator = new PropertyValidator();

            return validator.Validate(property);
          
        }
        public ValidationResult ValidatePropertyCode(string propertyCode)
        {

            var validator = new PropertyCodeValidator();
            return validator.Validate(propertyCode);
           

        }

        public ValidationResult ValidateTenant(Tenant tenant)
        {
            var validator = new TenantValidator();
            return  validator.Validate(tenant);
      
        }

        public ValidationResult ValidatePropertyUnit(string propertyUnit)
        {
            var validator = new UnitNumberValidator();
            return  validator.Validate(propertyUnit);
           
        }

        public ValidationResult ValidatePersonContactInfo(PersonContactInfo personContactInfo)
        {
            var validator = new PersonContactInfoValidator();
           return  validator.Validate(personContactInfo);
          }


    }
}
