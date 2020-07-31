using System;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IUserAuthorizationService
    {
        
        LoggedUser LoggedUser { get; }
        LoggedUser SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode=null, string unitNumber = null );
        void SetUser(LoggedUser loggedUser);
        void Check(Func<bool> action);
        bool IsRegisteredTenant(string propCode, string tenantUnit = null);
        bool IsLoggedTenant();
        bool IsRegisteredSuperintendent(string propCode=null);
        bool IsRegisteredWorker(string email = null);
        bool IsUserCommand(Type getType);
    }
}