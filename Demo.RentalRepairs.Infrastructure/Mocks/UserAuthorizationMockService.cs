using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthorizationMockService : IUserAuthorizationService
    {
        public UserAuthorizationMockService()
        {
           
        }

      

        public LoggedUser LoggedUser => new LoggedUser("");

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress)
        {
           return new LoggedUser("");
        }

        public void SetUser(LoggedUser loggedUser)
        {
            
        }

        public void UserCanChangeTenantRequestStatus(TenantRequestStatusEnum newStatus)
        {
            
        }

        public void UserCanChangeTenantRequestStatus(string propCode, string tenantUnit,
            TenantRequestStatusEnum newStatus)
        {
            
        }

        public void UserCanGetListOfProperties()
        {
            
        }

        public void UserCanGetListOfPropertyTenants(string propertyCode)
        {
            
        }

        public void UserCanGetListOfTenantRequests()
        {
           
        }

        public void UserCanGetListOfTenantRequests(string propertyCode, string tenantUnit)
        {
            
        }

        public void UserCanGetPropertyDetails(string propCode)
        {
            
        }

        public void UserCanRegisterProperty()
        {
            
        }

        public void UserCanRegisterTenant()
        {
           
        }

        public void UserCanRegisterTenant(string unitNumber)
        {
            
        }

        public void UserCanRegisterTenantRequest(string propCode, string tenantUnit)
        {
            
        }

        public void UserCanGetTenantDetails(string propertyCode, string propertyUnit)
        {
            
        }
    }
}
