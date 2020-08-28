using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface ISecurityService
    {
        Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password);
        Task<OperationResult > SignInUser(string email, string password, bool rememberMe);
        object SigninResult { get; set; }
        Task<UserClaims> GetLoggedUserClaims(string email);
        Task SetLoggedUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber);

        Task LogoutUser();
    }

    
}