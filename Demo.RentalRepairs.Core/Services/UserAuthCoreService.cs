using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserAuthCoreService : IUserAuthCoreService
    {
        private readonly IPropertyRepository _propertyRepository;
    
        private  LoggedUser _loggedUser;
        public UserAuthCoreService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
         
        }

        public LoggedUser  LoggedUser => _loggedUser;

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress)
        {
            switch (userRole)
            {
                case UserRolesEnum.Superintendent:
                    var prop = _propertyRepository.FindPropertyByLoginEmail(emailAddress);
                    // if not found, property is not registered yet
                    _loggedUser = prop == null ? new LoggedUser(emailAddress, UserRolesEnum.Superintendent) : new LoggedUser(emailAddress, UserRolesEnum.Superintendent, propCode: prop.Code);
                    break;
                case UserRolesEnum.Tenant:
                    var tenant = _propertyRepository.FindTenantByLoginEmail(emailAddress);
                    // if not found, tenant is not registered yet
                    _loggedUser = tenant == null ? new LoggedUser(emailAddress, UserRolesEnum.Tenant) : new LoggedUser(emailAddress, UserRolesEnum.Tenant, propCode: tenant.PropertyCode, unitNumber: tenant.UnitNumber);
                    break;
                case UserRolesEnum.Worker:
                    var worker = _propertyRepository.FindWorkerByLoginEmail(emailAddress);
                    // if not found, worker is not registered yet
                    _loggedUser = worker == null
                        ? new LoggedUser(emailAddress, UserRolesEnum.Worker)
                        : new LoggedUser(emailAddress, UserRolesEnum.Worker) { };
                    break;

                default:
                    _loggedUser = new LoggedUser(emailAddress, userRole);
                    break;
            }

            return _loggedUser;
        }

        public void SetUser(LoggedUser loggedUser)
        {
            _loggedUser = loggedUser;
        }
        public void VerifyUserAuthorizedFor_ListOfProperties()
        {

            if (_loggedUser.UserRole != UserRolesEnum.Administrator)
            {
                throw new DomainAuthorizationException("view_list_of_properties_is_not_allowed", "Only administrator is allowed to list all properties");
            }
        }

        public void VerifyUserAuthorizedFor_PropertyDetails(string propCode)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;

            if (_loggedUser.UserRole == UserRolesEnum.Superintendent || _loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (propCode == _loggedUser.PropCode)
                {
                    return;
                }

                throw new DomainAuthorizationException("only_assigned_property_can_be_viewed",
                    "only assigned property can be viewed");
            }
            throw new DomainAuthorizationException("access_is_not_allowed", "Not enough permissions for viewing property info");
        }

        public void VerifyUserAuthorizedFor_RegisterProperty()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && string.IsNullOrEmpty(_loggedUser.PropCode))
            {
                return;
            }
            throw new DomainAuthorizationException("add_property_is_not_allowed", "Only superintendent is allowed to register new property");
        }

        public void VerifyUserAuthorizedFor_ListOfPropertyTenants(string propertyCode)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && propertyCode == _loggedUser.PropCode)
            {
                return;
            }
            throw new DomainAuthorizationException("view_property_tenants_not_allowed", "No permissions to view property tenants");
        }

        public void VerifyUserAuthorizedFor_RegisterTenant(string propertyCode, string unitNumber)
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
                throw new DomainAuthorizationException("view_property_tenants_not_allowed", "No permissions to add a property tenant");
            }
        }

        public void VerifyUserAuthorizedFor_TenantDetails(string propertyCode, string propertyUnit)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Administrator)
                return;
            if (_loggedUser.UserRole == UserRolesEnum.Tenant && _loggedUser.PropCode == propertyCode &&
                _loggedUser.UnitNumber == propertyUnit)
                return;
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && _loggedUser.PropCode == propertyCode)
                return;

            throw new DomainAuthorizationException("view_property_tenant_details_not_allowed", "No permissions to view property tenant details");
        }

        public void VerifyUserAuthorizedFor_ListOfTenantRequests(string propertyCode, string tenantUnit)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (_loggedUser.PropCode == propertyCode && _loggedUser.UnitNumber == tenantUnit)
                {
                    return;
                }

            }
            throw new DomainAuthorizationException("user_not_allowed_to_view_tenant_requests", "No permissions to view  tenant requests");
        }

        public void VerifyUserAuthorizedFor_RegisterTenantRequest(string propCode, string tenantUnit)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (_loggedUser.PropCode == propCode && _loggedUser.UnitNumber == tenantUnit)
                {
                    return;
                }

            }
            throw new DomainAuthorizationException("user_not_allowed_to_register_request", "No permissions to register request");
        }

        public void VerifyUserAuthorizedFor_ChangeTenantRequestStatus(string propCode, string tenantUnit, TenantRequestStatusEnum newStatus)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                if (_loggedUser.PropCode != propCode)
                {
                    throw new DomainAuthorizationException("property_not_assigned_to_super", "No permissions to change status for this request");
                }

                if (newStatus == TenantRequestStatusEnum.WorkScheduled ||
                    newStatus == TenantRequestStatusEnum.RequestRejected || newStatus == TenantRequestStatusEnum.Closed)
                    return;

                throw new DomainAuthorizationException("super_cannot_change_request_status", "Superintendent has no permissions to change request status");

            }
            else if (_loggedUser.UserRole == UserRolesEnum.Worker)
            {
                //if (UserObject.PropCode != propCode)
                //{
                //    throw new SecurityException("property_not_assigned_to_worker", "No permissions to change status for this request");
                //}
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
    }
}
