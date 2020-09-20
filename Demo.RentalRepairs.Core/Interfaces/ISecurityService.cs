using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface ISecurityService
    {
        object SigninResult { get; set; }
        Task<OperationResult> RegisterUser( string email, string password);
        Task<OperationResult > SignInUser(string email, string password, bool rememberMe);
        Task<UserClaimsService> GetUserClaims(object principle);
        Task SetUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber);
        Task LogoutUser();
    }

    
}