using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public abstract class UserAuthorizationService : IUserAuthorizationService
    {
        protected   UserClaimsService IntLoggedUser;
        

        public UserClaimsService LoggedUser => IntLoggedUser;

        public abstract Task<OperationResult> SignInUser(string email, string password, bool rememberMe);
        public abstract  Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password);
        public abstract Task<UserClaimsService> GetUserClaims(string emailLogin);
        public abstract  Task SetUserClaims(string email, UserRolesEnum userRole, string propCode, string userNumber);
        public abstract string GetLoginFromClaimsPrinciple(ClaimsPrincipal principle);
        public object SigninResult { get; set; }

        public async Task<UserClaimsService> GetUserClaims(ClaimsPrincipal principle)
        {
            return await GetUserClaims(GetLoginFromClaimsPrinciple(principle));
        }
               


    }
}
