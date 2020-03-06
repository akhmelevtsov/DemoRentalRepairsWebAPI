using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly INotifyPartiesService _notifyPartiesService;
        private readonly IUserAuthCoreService _authService;
        private readonly PropertyDomainService _propertyDomainService = new PropertyDomainService();

        private readonly ValidationService _validationService = new ValidationService();
      
       
        
        public PropertyService(IPropertyRepository propertyRepository, INotifyPartiesService notifyPartiesService, IUserAuthCoreService authorizationService)
        {

            _propertyRepository = propertyRepository;
            _notifyPartiesService = notifyPartiesService;
            _authService = authorizationService;
        }

     
        public IEnumerable<Property> GetAllProperties()
        {
            _authService.VerifyUserAuthorizedFor_ListOfProperties();
            return _propertyRepository.GetAllProperties();
        }

        public Property GetPropertyByCode(string propCode)
        {
           _authService.VerifyUserAuthorizedFor_PropertyDetails(propCode);
            _validationService.ValidatePropertyCode(propCode);

            var prop =  _propertyRepository.GetPropertyByCode(propCode);
           
            return prop;
        }

        public Property AddProperty(string name, string propertyCode, PropertyAddress propertyAddress, string phoneNumber, PersonContactInfo superintendentInfo, List<string> units)
        {
           _authService.VerifyUserAuthorizedFor_RegisterProperty();
            _validationService.ValidatePropertyCode(propertyCode);
          
            var prop = _propertyDomainService.CreateProperty(name, propertyCode, propertyAddress, phoneNumber, superintendentInfo, units);
            prop.LoginEmail =_authService.LoggedUser.Login; 
            _propertyRepository.AddProperty(prop);
            return prop;
        }

        // tenants
        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
           _authService.VerifyUserAuthorizedFor_ListOfPropertyTenants(propertyCode);
            _validationService.ValidatePropertyCode(propertyCode);
            return _propertyRepository.GetPropertyTenants(propertyCode);
        }

        public Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber)
        {
           _authService.VerifyUserAuthorizedFor_RegisterTenant(propertyCode, unitNumber );

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(unitNumber);
            _validationService.ValidatePersonContactInfo(contactInfo);

            //var property = GetPropertyByCode(propertyCode);
            var property = _propertyRepository.GetPropertyByCode(propertyCode);
            var tenant = _propertyDomainService .AddTenant(property, contactInfo, unitNumber);
            tenant.LoginEmail =_authService.LoggedUser.Login; 
            _propertyRepository.AddTenant(tenant);
            return tenant;

        }
        public Tenant GetTenantByPropertyUnit(string propertyCode, string propertyUnit)
        {
           _authService.VerifyUserAuthorizedFor_TenantDetails(propertyCode, propertyUnit);

            _validationService.ValidatePropertyCode(propertyCode);
            var tenant = _propertyRepository.GetTenantByUnitNumber(propertyUnit, propertyCode);
            if (tenant == null)
                Tenant.NotFoundException(propertyUnit, propertyCode);
            return tenant;

        }
        //Requests
        public IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit)
        {
           _authService.VerifyUserAuthorizedFor_ListOfTenantRequests(propertyCode, tenantUnit);
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
           _authService.VerifyUserAuthorizedFor_RegisterTenantRequest(propCode, tenantUnit);
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
           _authService.VerifyUserAuthorizedFor_ChangeTenantRequestStatus(propCode, tenantUnit, newStatus);
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
