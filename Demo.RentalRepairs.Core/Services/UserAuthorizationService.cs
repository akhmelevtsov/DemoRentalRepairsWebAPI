using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly DomainValidationService _validationService = new DomainValidationService();

        private LoggedUser _loggedUser;
        public UserAuthorizationService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;

        }

        public LoggedUser LoggedUser => _loggedUser;

 
        public void SetUser(LoggedUser loggedUser)
        {
            _loggedUser = loggedUser;
        }
        public void UserCanGetListOfProperties()
        {

            if (_loggedUser.UserRole != UserRolesEnum.Administrator)
            {
                throw new DomainAuthorizationException("view_list_of_properties_is_not_allowed", "Only administrator is allowed to list all properties");
            }
        }

        public void UserCanGetPropertyDetails(string propCode)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;

            if (_loggedUser.UserRole == UserRolesEnum.Superintendent || _loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                _validationService.ValidatePropertyCode(propCode);
                if (propCode == _loggedUser.PropCode)
                {
                    return;
                }

                throw new DomainAuthorizationException("only_assigned_property_can_be_viewed",
                    "only assigned property can be viewed");
            }
            throw new DomainAuthorizationException("access_is_not_allowed", "Not enough permissions for viewing property info");
        }

        public void UserCanRegisterProperty()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && string.IsNullOrEmpty(_loggedUser.PropCode))
            {
                return;
            }
            throw new DomainAuthorizationException("add_property_is_not_allowed", "Only superintendent is allowed to register new property");
        }

        public void UserCanGetListOfPropertyTenants(string propertyCode)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;

            if (_loggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                _validationService.ValidatePropertyCode(propertyCode);
                if (propertyCode == _loggedUser.PropCode)
                    return;
            }
            throw new DomainAuthorizationException("view_property_tenants_not_allowed", "No permissions to view property tenants");
        }

        public void UserCanRegisterTenant()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (string.IsNullOrEmpty(_loggedUser.PropCode) && string.IsNullOrEmpty(_loggedUser.UnitNumber))
                {
                    return;

                }
                throw new DomainAuthorizationException("tenant_already_assigned", "No permissions to assign tenant to property more then once");
            }
            else
            {
                throw new DomainAuthorizationException("register_property_tenants_not_allowed", "No permissions to register property tenant");
            }
        }

        public void UserCanRegisterWorker()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Worker )
            {
                return;
            }
            throw new DomainAuthorizationException("register_worker_is_not_allowed", "only worker role can register worker");
        }

        public void UserCanGetListOfAllWorkers()
        {
            if (_loggedUser.UserRole != UserRolesEnum.Superintendent   )
            {
                throw new DomainAuthorizationException("view_list_of_workers_is_not_allowed", "Only superintendent is allowed to list all workers");
            }
        }

        public void UserCanGetTenantDetails(string propertyCode, string propertyUnit)
        {

            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;



            if (_loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                _validationService.ValidatePropertyCode(propertyCode);
                _validationService.ValidatePropertyUnit(propertyUnit);

                if (_loggedUser.PropCode == propertyCode && _loggedUser.UnitNumber == propertyUnit) return;
            }

            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && _loggedUser.PropCode == propertyCode)
                return;

            throw new DomainAuthorizationException("view_property_tenant_details_not_allowed", "No permissions to view property tenant details");
        }

  

        public void UserCanGetListOfTenantRequests()
        {
            if (_loggedUser.UserRole != UserRolesEnum.Tenant)
            {
                throw new DomainAuthorizationException("user_role_not_allowed_to_view_tenant_requests",
                    "No permissions to view  tenant requests");
            }
        }

        public void UserCanGetListOfTenantRequests(string propertyCode, string tenantUnit)
        {
             UserCanGetListOfTenantRequests();

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            if (_loggedUser.PropCode == propertyCode && _loggedUser.UnitNumber == tenantUnit)
            {
                return;
            }
            throw new DomainAuthorizationException("user_not_allowed_to_view_tenant_requests",
                "No permissions to view  tenant requests");

        }

        public void UserCanRegisterTenantRequest(string propCode, string tenantUnit)
        {

            if (_loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                _validationService.ValidatePropertyCode(propCode);
                _validationService.ValidatePropertyUnit(tenantUnit);
                if (_loggedUser.PropCode == propCode && _loggedUser.UnitNumber == tenantUnit)
                {
                    return;
                }

            }
            throw new DomainAuthorizationException("user_not_allowed_to_register_request", "No permissions to register request");
        }

        public void UserCanChangeTenantRequestStatus(TenantRequestStatusEnum newStatus)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                if (newStatus == TenantRequestStatusEnum.WorkScheduled ||
                    newStatus == TenantRequestStatusEnum.RequestRejected || newStatus == TenantRequestStatusEnum.Closed)
                    return;

                throw new DomainAuthorizationException("super_cannot_change_request_status", "Superintendent has no permissions to change request status");

            }
            else if (_loggedUser.UserRole == UserRolesEnum.Worker)
            {

                if (newStatus == TenantRequestStatusEnum.WorkCompleted ||
                    newStatus == TenantRequestStatusEnum.WorkIncomplete)
                    return;

                throw new DomainAuthorizationException("worker_cannot_change_request_status", "Worker has no permissions to change request status");

            }
            else
            {
                throw new DomainAuthorizationException("user_not_allowed_to_change_request_status", "No permissions to change request status");
            }

        }
        public void UserCanChangeTenantRequestStatus(string propCode, string tenantUnit, TenantRequestStatusEnum newStatus)
        {
            UserCanChangeTenantRequestStatus(newStatus);

            //_validationService.ValidatePropertyUnit(tenantUnit);

            if (_loggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                _validationService.ValidatePropertyCode(propCode);
                if (_loggedUser.PropCode != propCode)
                {
                    throw new DomainAuthorizationException("property_not_assigned_to_super", "No permissions to change status for this request");
                }
            }


        }
    }
}
