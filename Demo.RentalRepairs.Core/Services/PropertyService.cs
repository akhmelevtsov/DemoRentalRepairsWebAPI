using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly INotifyPartiesService _notifyPartiesService;
        private readonly PropertyDomainService _propertyDomainService = new PropertyDomainService();

        private readonly ValidationService _validationService = new ValidationService();

        public PropertyService(IPropertyRepository propertyRepository, INotifyPartiesService notifyPartiesService)
        {
            _propertyRepository = propertyRepository;
            _notifyPartiesService = notifyPartiesService;
        }

        // property
        public IEnumerable<Property> GetAllProperties()
        {
            return _propertyRepository.GetAllProperties();
        }

        public Property GetPropertyByCode(string propCode)
        {
            _validationService.ValidatePropertyCode(propCode);

            var prop =  _propertyRepository.GetPropertyByCode(propCode);
           
            return prop;
        }

        public Property AddProperty(string name, string propertyCode, PropertyAddress propertyAddress, string phoneNumber, PersonContactInfo superintendentInfo, List<string> units)
        {
            _validationService.ValidatePropertyCode(propertyCode);
          
            var prop = _propertyDomainService.CreateProperty(name, propertyCode, propertyAddress, phoneNumber, superintendentInfo, units);
            _propertyRepository.AddProperty(prop);
            return prop;
        }

        // tenants
        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            _validationService.ValidatePropertyCode(propertyCode);
            return _propertyRepository.GetPropertyTenants(propertyCode);
        }

        public Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber)
        {

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(unitNumber);
            _validationService.ValidatePersonContactInfo(contactInfo);

            var property = GetPropertyByCode(propertyCode);

            var tenant = _propertyDomainService .AddTenant(property, contactInfo, unitNumber);
            _propertyRepository.AddTenant(tenant);
            return tenant;

        }
        public Tenant GetTenantByPropertyUnit(string propertyCode, string propertyUnit)
        {
            _validationService.ValidatePropertyCode(propertyCode);
            var tenant = _propertyRepository.GetTenantByUnitNumber(propertyUnit, propertyCode);
            if (tenant == null)
                Tenant.NotFoundException(propertyUnit, propertyCode);
            return tenant;

        }
        //Requests
        public IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit)
        {
            var retList = new List<TenantRequest>();
            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propertyCode);
            if (tenant != null)
                retList =  _propertyRepository.GetTenantRequests(tenant.Id).ToList() ;
            else
                Tenant.NotFoundException(tenantUnit, propertyCode);
            return retList;
        }
        public TenantRequest RegisterTenantRequest(string propCode, string tenantUnit , TenantRequestDoc tenantRequestDoc)
        {
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var  tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propCode);
            if (tenant == null)
                Tenant.NotFoundException(tenantUnit, propCode);

            var tenantRequest = _propertyDomainService.RegisterTenantRequest(tenant, tenantRequestDoc);
            _propertyRepository.AddTenantRequest(tenantRequest);
            _notifyPartiesService.NotifyRequestStatusChange(tenantRequest.BuildMessage());
            return tenantRequest;
        }

        public TenantRequest ChangeRequestStatus(string propCode, string tenantUnit, string requestCode,
            TenantRequestStatusEnum newStatus, TenantRequestBaseDoc tenantRequestBaseDoc)
        {
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenantRequest = _propertyRepository.GetTenantRequest(propCode, tenantUnit, requestCode);
            tenantRequest = tenantRequest.ChangeStatus(newStatus, tenantRequestBaseDoc);
            _propertyRepository.UpdateTenantRequest(tenantRequest);
            _notifyPartiesService.NotifyRequestStatusChange(tenantRequest.BuildMessage());
            return tenantRequest;
        }

    
        

    }
}
