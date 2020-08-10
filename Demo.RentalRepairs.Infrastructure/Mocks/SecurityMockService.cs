using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class SecurityMockService : ISecurityService 
    {
        public async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            await Task.CompletedTask;
            return new OperationResult() {Succeeded = true};
        }

        public async Task<OperationResult> SignInUser(string email, string password, bool rememberMe)
        {
            await Task.CompletedTask;
            return new OperationResult() { Succeeded = true };
        }

        public object SigninResult { get; set; }

        public async Task<UserClaims> GetLoggedUserClaims(string email)
        {
            await Task.CompletedTask;
            return new UserClaims(email,UserRolesEnum.Anonymous , "", "");
        }

        public async Task SetLoggedUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber)
        {
            await Task.CompletedTask;
        }

        public async Task LogoutUser()
        {
            await Task.CompletedTask;
        }
    }
}
