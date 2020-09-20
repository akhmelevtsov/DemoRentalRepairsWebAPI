using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IUserAuthorizationService
    {
        
        UserClaimsService LoggedUser { get; }
        Task<OperationResult> SignInUser(string email, string password, bool rememberMe);
        Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password);
        Task<UserClaimsService> GetUserClaims(string emailLogin);
        Task<UserClaimsService> GetUserClaims(ClaimsPrincipal principle);
        Task SetUserClaims(string email, UserRolesEnum userRole, string propCode, string userNumber);

        string GetLoginFromClaimsPrinciple(ClaimsPrincipal principle);
        object SigninResult { get; set; }

    }
}