using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IUserAuthorizationService
    {
        
        LoggedUser LoggedUser { get; }
        //LoggedUser SetUser(UserRolesEnum userRole, string emailAddress);
        void SetUser(LoggedUser loggedUser);
        void UserCanChangeTenantRequestStatus(TenantRequestStatusEnum newStatus);
        void UserCanChangeTenantRequestStatus(string propCode, string tenantUnit, TenantRequestStatusEnum newStatus);
        void UserCanGetListOfProperties();
        void UserCanGetListOfPropertyTenants(string propertyCode);
        void UserCanGetListOfTenantRequests();
        void UserCanGetListOfTenantRequests(string propertyCode, string tenantUnit);
        void UserCanGetPropertyDetails(string propCode);
        void UserCanRegisterProperty();
        void UserCanRegisterTenant();
        void UserCanRegisterTenantRequest(string propCode, string tenantUnit);
        void UserCanGetTenantDetails(string propertyCode, string propertyUnit);
        void UserCanRegisterWorker();
        void UserCanGetListOfAllWorkers();
    }
}