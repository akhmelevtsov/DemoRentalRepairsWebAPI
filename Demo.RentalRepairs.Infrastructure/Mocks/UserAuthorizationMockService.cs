using System;
using System.Threading.Tasks;
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

        public UserClaims LoggedUser => new UserClaims("");

        public UserClaims SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode, string unitNumber)
        {
           return new UserClaims(emailAddress, userRole,propertyCode , unitNumber );
        }

        public void SetUser(UserClaims loggedUser)
        {
            
        }

        public async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            await Task.CompletedTask;
            return new OperationResult() {Succeeded = true};
        }

        public Task<UserClaims> GetUserClaims(string email)
        {
            throw new NotImplementedException();
        }

        public void Check(Func<bool> action)
        {
           
        }

        public bool IsRegisteredTenant(string propCode, string tenantUnit)
        {
            return true;
        }

        public bool IsAuthenticatedTenant()
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

        public bool IsAuthenticatedSuperintendent()
        {
            return true;
        }

        public async Task AddUserClaims(string propCode, string userNumber)
        {
            await Task.CompletedTask;
        }
    }
}
