using System;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IUserAuthorizationService
    {
        
        UserClaims LoggedUser { get; }
        UserClaims SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode=null, string unitNumber = null );
        void SetUser(UserClaims loggedUser);
        Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password);
        Task<UserClaims> GetUserClaims(string email);
        Task SetUserClaims(UserRolesEnum userRole, string propCode, string userNumber);
        void Check(Func<bool> action);
        bool IsRegisteredTenant(string propCode, string tenantUnit = null);
        bool IsAuthenticatedTenant();
        bool IsRegisteredSuperintendent(string propCode=null);
        bool IsRegisteredWorker(string email = null);
        bool IsUserCommand(Type getType);
        bool IsAuthenticatedSuperintendent();
        bool IsAnonymousUser();
    }
}