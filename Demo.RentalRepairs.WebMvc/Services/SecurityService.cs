using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.WebMvc.Data;
using Microsoft.AspNetCore.Identity;

namespace Demo.RentalRepairs.WebMvc.Services
{
    public class SecurityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAuthorizationService _userAuthCoreService;

        public SecurityService(UserManager<ApplicationUser> userManager, IUserAuthorizationService userAuthCoreService)
        {
            _userManager = userManager;
            _userAuthCoreService = userAuthCoreService;
        }
        public async Task<LoggedUser> GetLoggedTenant(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            var claims = await _userManager.GetClaimsAsync(user);
            var pc = claims.FirstOrDefault(x => x.Type == SecurityClaims.PropertyCode.ToString());
            var un = claims.FirstOrDefault(x => x.Type == SecurityClaims.UnitNumber.ToString());
            var propCode = pc == null ? "" : pc.Value;
            var unitNumber = un == null ? "" : un.Value;

            var loggedUser = new LoggedUser(user.Email, UserRolesEnum.Tenant, propCode, unitNumber);
            _userAuthCoreService.SetUser(loggedUser);

            return loggedUser;
        }
        public  async Task AddTenantClaims(ClaimsPrincipal claimsPrincipal, string propCode, string unitNumber)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.PropertyCode.ToString(), propCode));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.UnitNumber.ToString(), unitNumber));
            await _userManager.UpdateAsync(user);
        }

        public async Task<LoggedUser> GetLoggedSuperintendent(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            var claims = await _userManager.GetClaimsAsync(user);
            var pc = claims.FirstOrDefault(x => x.Type == SecurityClaims.PropertyCode.ToString());
            var propCode = pc == null ? "" : pc.Value;

            var loggedUser = new LoggedUser(user.Email, UserRolesEnum.Superintendent , propCode);
            _userAuthCoreService.SetUser(loggedUser);
            return loggedUser;
        }
        public async Task AddSuperintendentClaims(ClaimsPrincipal claimsPrincipal, string propCode)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.PropertyCode.ToString(), propCode));
            await _userManager.UpdateAsync(user);
        }

        public async Task<LoggedUser> GetLoggedWorker(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            //var claims = await _userManager.GetClaimsAsync(user);
            //var pc = claims.FirstOrDefault(x => x.Type == SecurityClaims.PropertyCode.ToString());
            //var propCode = pc == null ? "" : pc.Value;
            var loggedUser = new LoggedUser(user.Email, UserRolesEnum.Worker );
            _userAuthCoreService.SetUser(loggedUser);
            return loggedUser;
        }
        public async Task AddWorkerClaims(ClaimsPrincipal claimsPrincipal)
        {
            await Task.CompletedTask;
            //var user = await _userManager.GetUserAsync(claimsPrincipal);
            //await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.PropertyCode.ToString(), propCode));
            //await _userManager.UpdateAsync(user);
        }
    }
}
