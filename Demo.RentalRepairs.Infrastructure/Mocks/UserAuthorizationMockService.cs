using System;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthorizationMockService : IUserAuthorizationService
    {
        public UserAuthorizationMockService()
        {
           
        }

        public LoggedUser LoggedUser => new LoggedUser("");

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode, string unitNumber)
        {
           return new LoggedUser(emailAddress, userRole,propertyCode , unitNumber );
        }

        public void SetUser(LoggedUser loggedUser)
        {
            
        }

        public void Check(Func<bool> action)
        {
           
        }

        public bool IsRegisteredTenant(string propCode, string tenantUnit)
        {
            return true;
        }

        public bool IsLoggedTenant()
        {
            return true;
        }

        public bool IsRegisteredSuperintendent(string propCode)
        {
            return true;
        }

        public bool IsRegisteredWorker(string email=null)
        {
            return true;
        }

        public bool IsUserCommand(Type getType)
        {
            return true;
        }
    }
}
