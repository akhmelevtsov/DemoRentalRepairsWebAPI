using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation.Results;

namespace Demo.RentalRepairs.Domain.Services
{
    public class DomainValidationService
    {
        private readonly ValidationRulesService _validationRulesService = new ValidationRulesService();

        public  void ValidateProperty( Property property)
        {

            var results = _validationRulesService.ValidateProperty(property);
            if (!results.IsValid)
            {
                throw new DomainValidationException("create_property_validation", results.Errors);
            }
        }
        public  void ValidatePropertyCode(string propertyCode)
        {

            var results = _validationRulesService.ValidatePropertyCode(propertyCode);
            if (!results.IsValid)
            {
                throw new DomainValidationException("property_code_validation", results.Errors);
            }

        }

        public  void ValidateTenant( Tenant tenant)
        {
            
            var results = _validationRulesService.ValidateTenant(tenant);
            if (!results.IsValid)
            {
                throw new DomainValidationException("add_tenant_validation", results.Errors);
            }
        }

        public  void ValidatePropertyUnit(string propertyUnit)
        {
          
            var results = _validationRulesService.ValidatePropertyUnit(propertyUnit);
            if (!results.IsValid)
            {
                throw new DomainValidationException("property_unit_validation", results.Errors);
            }
        }

        public  void ValidatePersonContactInfo(PersonContactInfo personContactInfo)
        {
            
            var results = _validationRulesService.ValidatePersonContactInfo(personContactInfo);
            if (!results.IsValid)
            {
                throw new DomainValidationException("personContactInfo_validation", results.Errors);
            }
        }
    }
}
