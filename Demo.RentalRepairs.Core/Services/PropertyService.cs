using System.Collections.Generic;
using System.Linq;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
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
        private readonly IUserAuthorizationService _authService;
      

        private readonly DomainValidationService _validationService = new DomainValidationService();

       

        public PropertyService(IPropertyRepository propertyRepository, INotifyPartiesService notifyPartiesService, IUserAuthorizationService authorizationService)
        {

            _propertyRepository = propertyRepository;
            _notifyPartiesService = notifyPartiesService;
            _authService = authorizationService;
        }

        public LoggedUser GetUser(UserRolesEnum userRole, string emailAddress)
        {
            LoggedUser loggedUser = null;
            switch (userRole)
            {
                case UserRolesEnum.Superintendent:
                    var prop = _propertyRepository.FindPropertyByLoginEmail(emailAddress);
                    // if not found, property is not registered yet
                    loggedUser = prop == null ? new LoggedUser(emailAddress, UserRolesEnum.Superintendent) : new LoggedUser(emailAddress, UserRolesEnum.Superintendent, propCode: prop.Code);
                    break;
                case UserRolesEnum.Tenant:
                    var tenant = _propertyRepository.FindTenantByLoginEmail(emailAddress);
                    // if not found, tenant is not registered yet
                    loggedUser = tenant == null ? new LoggedUser(emailAddress, UserRolesEnum.Tenant) : new LoggedUser(emailAddress, UserRolesEnum.Tenant, propCode: tenant.PropertyCode, unitNumber: tenant.UnitNumber);
                    break;
                case UserRolesEnum.Worker:
                    var worker = _propertyRepository.FindWorkerByLoginEmail(emailAddress);
                    // if not found, worker is not registered yet
                    loggedUser = worker == null
                        ? new LoggedUser(emailAddress, UserRolesEnum.Worker)
                        : new LoggedUser(emailAddress, UserRolesEnum.Worker) { };
                    break;

                default:
                    loggedUser = new LoggedUser(emailAddress, userRole);
                    break;
            }

            return loggedUser;
        }

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress)
        {
            var user = this.GetUser(userRole, emailAddress);
            _authService.SetUser( user);
            return user;
        }
        public IEnumerable<Property> GetAllProperties()
        {
            _authService.UserCanGetListOfProperties();
            return _propertyRepository.GetAllProperties();
        }

 

        public Property GetPropertyByCode(string propCode)
        {
            _validationService.ValidatePropertyCode(propCode);
            _authService.UserCanGetPropertyDetails(propCode);
            var prop = _propertyRepository.GetPropertyByCode(propCode);

            return prop;
        }

        
        public Property AddProperty(PropertyInfo propertyInfo)
        {
            _authService.UserCanRegisterProperty();

            var prop = new Property(propertyInfo)
            {
                LoginEmail = _authService.LoggedUser.Login
            };
            _propertyRepository.AddProperty(prop);
            return prop;

        }
        // tenants
        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            _authService.UserCanGetListOfPropertyTenants(propertyCode);
            _validationService.ValidatePropertyCode(propertyCode);
            return _propertyRepository.GetPropertyTenants(propertyCode);
        }

        public Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber)
        {
            _authService.UserCanRegisterTenant();

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(unitNumber);
            _validationService.ValidatePersonContactInfo(contactInfo);

            var property = _propertyRepository.GetPropertyByCode(propertyCode);
            var tenant = property.AddTenant( contactInfo, unitNumber);
            tenant.LoginEmail = _authService.LoggedUser.Login;
            _propertyRepository.AddTenant(tenant);
            
            return tenant;

        }
        public Tenant GetTenantByPropertyUnit(string propertyCode, string propertyUnit)
        {
            _authService.UserCanGetTenantDetails(propertyCode, propertyUnit);

            _validationService.ValidatePropertyCode(propertyCode);
            var tenant = _propertyRepository.GetTenantByUnitNumber(propertyUnit, propertyCode);

            return tenant;

        }
        //Requests
        public IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit)
        {
            _authService.UserCanGetListOfTenantRequests();
            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propertyCode);

            var retList = new List<TenantRequest>();

            if (tenant != null)
            {
                _authService.UserCanGetListOfTenantRequests(propertyCode, tenantUnit);
                retList = _propertyRepository.GetTenantRequests(tenant.Id).ToList();
            }

            return retList;
        }
        public TenantRequest RegisterTenantRequest(string propCode, string tenantUnit, TenantRequestDoc tenantRequestDoc)
        {
            _authService.UserCanRegisterTenantRequest(propCode, tenantUnit);
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propCode);
            if (tenant == null)
                return null;
            var tenantRequest = tenant.AddRequest( tenantRequestDoc);

            _propertyRepository.AddTenantRequest(tenantRequest);

            _notifyPartiesService.CreateAndSendEmail(tenantRequest);
        
            return tenantRequest;
        }

        public TenantRequest ChangeRequestStatus(string propCode, string tenantUnit, string requestCode,
            TenantRequestStatusEnum newStatus, TenantRequestBaseDoc tenantRequestBaseDoc)
        {
            _authService.UserCanChangeTenantRequestStatus(newStatus);
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenantRequest = _propertyRepository.GetTenantRequest(propCode, tenantUnit, requestCode);
            if (tenantRequest == null)
                return null;

            _authService.UserCanChangeTenantRequestStatus(propCode, tenantUnit, newStatus);

            tenantRequest = tenantRequest.ChangeStatus(newStatus, tenantRequestBaseDoc);

            _propertyRepository.UpdateTenantRequest(tenantRequest);

            _notifyPartiesService.CreateAndSendEmail(tenantRequest);

            return tenantRequest;
        }




    }
}
