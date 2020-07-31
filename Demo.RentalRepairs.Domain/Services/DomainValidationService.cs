using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Services
{
    public class DomainValidationService
    {
        private readonly ValidationRulesService _validationRulesService = new ValidationRulesService();

        public  void ValidateProperty( Property property)
        {

            var results = _validationRulesService.Validate(property);
            if (!results.IsValid)
            {
                throw new DomainValidationException("create_property_validation", results.Errors);
            }
        }
        public void Validate(AddPropertyCommand propertyInfo)
        {
            var results = _validationRulesService.Validate(propertyInfo);
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

        public  void Validate( Tenant tenant)
        {
            
            var results = _validationRulesService.Validate(tenant);
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

        public  void Validate(PersonContactInfo personContactInfo)
        {
            
            var results = _validationRulesService.Validate(personContactInfo);
            if (!results.IsValid)
            {
                throw new DomainValidationException("personContactInfo_validation", results.Errors);
            }
        }
        public void Validate(RegisterTenantRequestCommand tenantRequestCommand)
        {

            var results = _validationRulesService.Validate(tenantRequestCommand);
            if (!results.IsValid)
            {
                throw new DomainValidationException("tenantRequestDoc_validation", results.Errors);
            }
        }
        public void Validate(ScheduleServiceWorkCommand  serviceWorkOrder)
        {

            var results = _validationRulesService.Validate(serviceWorkOrder);
            if (!results.IsValid)
            {
                throw new DomainValidationException("serviceWorkOrder_validation", results.Errors);
            }
        }
        public void Validate(ReportServiceWorkCommand serviceWorkCommand)
        {

            var results = _validationRulesService.Validate(serviceWorkCommand);
            if (!results.IsValid)
            {
                throw new DomainValidationException("serviceWorkOrder_validation", results.Errors);
            }
        }

    }
}
