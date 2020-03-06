using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public interface IUserAuthCoreService
    {
        
        LoggedUser LoggedUser { get; }
        LoggedUser SetUser(UserRolesEnum userRole, string emailAddress);
        void SetUser(LoggedUser loggedUser);
        void VerifyUserAuthorizedFor_ChangeTenantRequestStatus(string propCode, string tenantUnit, TenantRequestStatusEnum newStatus);
        void VerifyUserAuthorizedFor_ListOfProperties();
        void VerifyUserAuthorizedFor_ListOfPropertyTenants(string propertyCode);
        void VerifyUserAuthorizedFor_ListOfTenantRequests(string propertyCode, string tenantUnit);
        void VerifyUserAuthorizedFor_PropertyDetails(string propCode);
        void VerifyUserAuthorizedFor_RegisterProperty();
        void VerifyUserAuthorizedFor_RegisterTenant(string propertyCode, string unitNumber);
        void VerifyUserAuthorizedFor_RegisterTenantRequest(string propCode, string tenantUnit);
        void VerifyUserAuthorizedFor_TenantDetails(string propertyCode, string propertyUnit);
    }
}