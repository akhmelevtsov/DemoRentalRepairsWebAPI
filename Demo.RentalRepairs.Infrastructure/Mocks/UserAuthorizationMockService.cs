using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthorizationMockService : UserAuthorizationService 
    {
        private readonly Dictionary <string, UserClaimsService> _claims = new Dictionary<string,UserClaimsService>();


       public override async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            await Task.CompletedTask;
            _claims.Add(email, new UserClaimsService(email, UserRolesEnum.Anonymous, "", ""));
            return new OperationResult() {Succeeded = true};
        }

        public override async Task<OperationResult> SignInUser(string email, string password, bool rememberMe)
        {
            await Task.CompletedTask;
            return new OperationResult() { Succeeded = true };
        }

        //public object SigninResult { get; set; }

 
        public override  async Task<UserClaimsService> GetUserClaims(string emailLogin)
        {
            await Task.CompletedTask;
            if (!_claims.ContainsKey((string)emailLogin))
                throw new Exception("User not found");
            var claim = _claims[(string)emailLogin];
            IntLoggedUser  = claim;
            return claim;
        }

        public override  async Task SetUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber)
        {
            await Task.CompletedTask;
            if (!_claims.ContainsKey(email ) )
                throw new Exception("User not logged in");
            _claims[email] = new UserClaimsService(email, userRole, propCode, unitNumber);

        }

        public override string GetLoginFromClaimsPrinciple(ClaimsPrincipal principle)
        {
            throw new NotImplementedException();
        }

        public async Task LogoutUser()
        {
            await Task.CompletedTask;
        }
    }
}
