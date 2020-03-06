using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthorizationMockService : IUserAuthCoreService
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

        public void VerifyUserAuthorizedFor_ChangeTenantRequestStatus(string propCode, string tenantUnit,
            TenantRequestStatusEnum newStatus)
        {
            
        }

        public void VerifyUserAuthorizedFor_ListOfProperties()
        {
            
        }

        public void VerifyUserAuthorizedFor_ListOfPropertyTenants(string propertyCode)
        {
            
        }

        public void VerifyUserAuthorizedFor_ListOfTenantRequests(string propertyCode, string tenantUnit)
        {
            
        }

        public void VerifyUserAuthorizedFor_PropertyDetails(string propCode)
        {
            
        }

        public void VerifyUserAuthorizedFor_RegisterProperty()
        {
            
        }

        public void VerifyUserAuthorizedFor_RegisterTenant(string propertyCode, string unitNumber)
        {
            
        }

        public void VerifyUserAuthorizedFor_RegisterTenantRequest(string propCode, string tenantUnit)
        {
            
        }

        public void VerifyUserAuthorizedFor_TenantDetails(string propertyCode, string propertyUnit)
        {
            
        }
    }
}
