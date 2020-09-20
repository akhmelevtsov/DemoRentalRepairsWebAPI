using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Demo.RentalRepairs.Infrastructure.Identity.AspNetCore
{
    public class AspNetIdentityService : UserAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ISecuritySignInService _securitySignInService;
        //public object SigninResult { get; set; }

        public AspNetIdentityService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ILogger<AspNetIdentityService> logger, ISecuritySignInService securitySignInService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _securitySignInService = securitySignInService;
        }
  
        public override  async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            //userRole = UserRolesEnum.Anonymous; // replace for now
            if (result.Succeeded)
            {
                var roleCheck = await _roleManager.RoleExistsAsync(UserRolesEnum.Anonymous.ToString());
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(UserRolesEnum.Anonymous.ToString()));
                }

                user = await _userManager.FindByEmailAsync(email);
                await _userManager.AddToRoleAsync(user, UserRolesEnum.Anonymous.ToString());


                _logger.LogInformation("User created a new account with password.");

                
                await _securitySignInService.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("User created a new account with password.");
                return new OperationResult() { Succeeded = true };
            }

            return new OperationResult()
            {
                Succeeded = false,
                ErrorsValueTuples = result.Errors.Select(x => new Tuple<string, string>(x.Code, x.Description)).ToList()
            };
        }
        public override async Task<OperationResult> SignInUser(string email, string password, bool rememberMe)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            //var  result = await _securitySignInService.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            SigninResult = result;
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                // Resolve the user via their email
                var user = await _userManager.FindByEmailAsync(email);
                // Get the roles for the user
                var roles = await _userManager.GetRolesAsync(user);
                //var claims = await _userManager.GetClaimsAsync(user);
                if (roles.Any() && Enum.TryParse(roles.FirstOrDefault(), out UserRolesEnum userRole))
                    return new OperationResult()
                    {
                        Succeeded = true,
                        UserRole = userRole
                    };

            }
            return new OperationResult() { Succeeded = false };
        }

        public override  async Task<UserClaimsService> GetUserClaims(string emailLogin)
        {
            if (string.IsNullOrEmpty(emailLogin))
            {
                //throw new NullReferenceException(nameof(emailClaim));
                IntLoggedUser = new UserClaimsService("",
                    UserRolesEnum.Anonymous,
                    "",
                    "");
                return IntLoggedUser;
            }

            var user = await _userManager.FindByEmailAsync(emailLogin);
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            if (!(roles.Any() && Enum.TryParse(roles.FirstOrDefault(), out UserRolesEnum  userRole)))
                throw new Exception();

            UserClaimsService loggedUser = new UserClaimsService(user.Email,
                userRole,
                "",
                "");


            switch (userRole)
            {
                case UserRolesEnum.Anonymous:
                    break;
                case UserRolesEnum.Tenant:
                    loggedUser = new UserClaimsService(user.Email,
                        userRole,
                        claims.FirstOrDefault(x => x.Type == SecurityClaims.PropertyCode.ToString())?.Value,
                        claims.FirstOrDefault(x => x.Type == SecurityClaims.UnitNumber.ToString())?.Value);

                    break;
                case UserRolesEnum.Superintendent:
                    loggedUser = new UserClaimsService(user.Email,
                        userRole,
                        claims.FirstOrDefault(x => x.Type == SecurityClaims.PropertyCode.ToString())?.Value,
                        "");

                    break;
                case UserRolesEnum.Administrator:
                    break;
                case UserRolesEnum.Worker:

                    break;
            }

            IntLoggedUser = loggedUser;
            return loggedUser;
        }

       

        public override async Task SetUserClaims(string email,UserRolesEnum userRole, string propCode, string unitNumber)
        {
            

            var user = await _userManager.FindByEmailAsync(email);

            var roleCheck = await _roleManager.RoleExistsAsync(userRole.ToString());
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(userRole.ToString()));
            }
            await _userManager.RemoveFromRoleAsync( user, UserRolesEnum.Anonymous.ToString());
            await _userManager .AddToRoleAsync(user, userRole.ToString());
            await _securitySignInService.SignInAsync(user, isPersistent: false);
            switch (userRole)
            {
                case UserRolesEnum.Anonymous:
                    break;
                case UserRolesEnum.Tenant:
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.PropertyCode.ToString(), propCode));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.UnitNumber.ToString(), unitNumber));
                    await _userManager.UpdateAsync(user);
                    break;
                case UserRolesEnum.Superintendent:
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(SecurityClaims.PropertyCode.ToString(), propCode));
                    await _userManager.UpdateAsync(user);

                    break;
                case UserRolesEnum.Administrator:
                    break;
                case UserRolesEnum.Worker:
                    break;
            }
        }

        public override string GetLoginFromClaimsPrinciple(ClaimsPrincipal claimsPrinciple)
        {
            if (claimsPrinciple == null) throw new ArgumentNullException(nameof(claimsPrinciple));

            return claimsPrinciple.Identity.Name;
        }

        public async Task LogoutUser()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }

       
    }
}
