using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Domain.Services
{
    public interface IUserAuthDomainService
    {
        LoggedUser LoggedUser { get; set; }

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