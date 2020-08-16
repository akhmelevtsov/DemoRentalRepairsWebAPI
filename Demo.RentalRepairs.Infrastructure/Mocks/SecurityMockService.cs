using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class SecurityMockService : ISecurityService
    {
        private readonly Dictionary <string, UserClaims> _claims = new Dictionary<string,UserClaims>();

        public async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            await Task.CompletedTask;
            _claims.Add(email, new UserClaims(email, userRole, "", ""));
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
            if (!_claims.ContainsKey(email))
                throw new Exception("User not found");
            var claim = _claims[email];
            return claim;
        }

        public async Task SetLoggedUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber)
        {
            await Task.CompletedTask;
            if (!_claims.ContainsKey(email ) )
                throw new Exception("User not logged in");
            _claims[email] = new UserClaims(email, userRole, propCode, unitNumber);

        }

        public async Task LogoutUser()
        {
            await Task.CompletedTask;
        }
    }
}
