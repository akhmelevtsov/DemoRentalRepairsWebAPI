using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Validators;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Domain.ValueObjects.Validators;
using FluentValidation.Results;

namespace Demo.RentalRepairs.Domain.Services
{
    public class ValidationRulesService
    {
        public  ValidationResult Validate  ( Property  property)
        {
            var validator = new PropertyValidator();

            return validator.Validate(property);
          
        }
        public ValidationResult Validate(AddPropertyCommand propertyInfo)
        {
            var validator = new AddPropertyCommandValidator();

            return validator.Validate(propertyInfo);
        }
        public ValidationResult ValidatePropertyCode(string propertyCode)
        {

            var validator = new PropertyCodeValidator();
            return validator.Validate(propertyCode);
           

        }

        public ValidationResult Validate(Tenant tenant)
        {
            var validator = new TenantValidator();
            return  validator.Validate(tenant);
      
        }

        public ValidationResult ValidatePropertyUnit(string propertyUnit)
        {
            if (propertyUnit == null)
                propertyUnit = string.Empty;
            var validator = new UnitNumberValidator();
            return  validator.Validate(propertyUnit);
           
        }

        public ValidationResult Validate(PersonContactInfo personContactInfo)
        {
            var validator = new PersonContactInfoValidator();
           return  validator.Validate(personContactInfo);
        }

        public ValidationResult Validate(RegisterTenantRequestCommand tenantRequestDoc)
        {
            var validator = new RegisterTenantRequestCommandValidator();
            return validator.Validate(tenantRequestDoc);
        }

        public ValidationResult Validate(ScheduleServiceWorkCommand  serviceWorkOrder)
        {
            var validator = new ScheduleServiceWorkCommandValidator();
            return validator.Validate(serviceWorkOrder);
        }
        public ValidationResult Validate(ReportServiceWorkCommand serviceWorkCommand)
        {
            var validator = new ReportServiceWorkCommandValidator();
            return validator.Validate(serviceWorkCommand);
        }

    }
}
