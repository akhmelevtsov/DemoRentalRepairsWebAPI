using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Services
{
    public class UserAuthDomainService : IUserAuthDomainService
    {
        public LoggedUser LoggedUser { get; set; }

        public UserAuthDomainService()
        {
            LoggedUser = new LoggedUser("");
        }

        public void VerifyUserAuthorizedFor_ListOfProperties()
        {
            
            if (LoggedUser.UserRole != UserRolesEnum.Administrator)
            {
                throw new DomainAuthorizationException("view_list_of_properties_is_not_allowed", "Only administrator is allowed to list all properties");
            }
        }

        public void VerifyUserAuthorizedFor_PropertyDetails(string propCode)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Administrator)
                return;

            if (LoggedUser.UserRole == UserRolesEnum.Superintendent || LoggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (propCode == LoggedUser.PropCode)
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
            if (LoggedUser.UserRole == UserRolesEnum.Superintendent && string.IsNullOrEmpty( LoggedUser.PropCode) )
            {
                return;
            }
            throw new DomainAuthorizationException("add_property_is_not_allowed", "Only superintendent is allowed to register new property");
        }

        public void VerifyUserAuthorizedFor_ListOfPropertyTenants(string propertyCode)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Administrator)
                return;
            if (LoggedUser.UserRole == UserRolesEnum.Superintendent && propertyCode == LoggedUser.PropCode )
            {
                return;
            }
            throw new DomainAuthorizationException("view_property_tenants_not_allowed", "No permissions to view property tenants");
        }

        public void VerifyUserAuthorizedFor_RegisterTenant(string propertyCode, string unitNumber)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (string.IsNullOrEmpty(LoggedUser.PropCode) && string.IsNullOrEmpty(LoggedUser.UnitNumber))
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
            if (LoggedUser.UserRole == UserRolesEnum.Administrator)
                return;
            if (LoggedUser.UserRole == UserRolesEnum.Tenant && LoggedUser.PropCode == propertyCode &&
                LoggedUser.UnitNumber == propertyUnit)
                return;
            if (LoggedUser.UserRole == UserRolesEnum.Superintendent && LoggedUser.PropCode == propertyCode)
                return;

            throw new DomainAuthorizationException("view_property_tenant_details_not_allowed", "No permissions to view property tenant details");
        }

        public void VerifyUserAuthorizedFor_ListOfTenantRequests(string propertyCode, string tenantUnit)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (LoggedUser.PropCode == propertyCode && LoggedUser.UnitNumber == tenantUnit)
                {
                    return;
                }

            }
            throw new DomainAuthorizationException("user_not_allowed_to_view_tenant_requests", "No permissions to view  tenant requests");
        }

        public void VerifyUserAuthorizedFor_RegisterTenantRequest(string propCode, string tenantUnit)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Tenant)
            {
                if (LoggedUser.PropCode == propCode && LoggedUser.UnitNumber == tenantUnit)
                {
                    return;
                }
              
            }
            throw new DomainAuthorizationException("user_not_allowed_to_register_request", "No permissions to register request");
        }

        public void VerifyUserAuthorizedFor_ChangeTenantRequestStatus(string propCode, string tenantUnit, TenantRequestStatusEnum newStatus)
        {
            if (LoggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                if (LoggedUser.PropCode != propCode)
                {
                    throw new DomainAuthorizationException("property_not_assigned_to_super","No permissions to change status for this request");
                }

                if (newStatus == TenantRequestStatusEnum.WorkScheduled ||
                    newStatus == TenantRequestStatusEnum.RequestRejected || newStatus == TenantRequestStatusEnum.Closed)
                    return;

                throw new DomainAuthorizationException("super_cannot_change_request_status", "Superintendent has no permissions to change request status");

            }
            else if (LoggedUser.UserRole == UserRolesEnum.Worker)
            {
                //if (UserObject.PropCode != propCode)
                //{
                //    throw new SecurityException("property_not_assigned_to_worker", "No permissions to change status for this request");
                //}
                if (newStatus == TenantRequestStatusEnum.WorkCompleted ||
                    newStatus == TenantRequestStatusEnum.WorkIncomplete )
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
