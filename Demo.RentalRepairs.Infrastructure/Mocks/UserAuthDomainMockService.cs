using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthDomainMockService : IUserAuthDomainService
    {
        public LoggedUser LoggedUser { get; set; }

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
