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
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using OperationResult = Demo.RentalRepairs.Core.Interfaces.OperationResult;

namespace Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C
{
    public class AzureAdB2CIdentityService : UserAuthorizationService
    {
        private readonly GraphServiceClient _graphClient;
        //private UserClaimsService _loggedUser;
        private readonly AzureAdB2CSettings _config;
        private IEnumerable<Claim> _claims;

        public AzureAdB2CIdentityService(IOptions<AzureAdB2CSettings> options)
        {
            // Read application settings from appsettings.json (tenant ID, app ID, client secret, etc.)
            _config = options.Value;
          
            // Initialize the client credential auth provider
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(_config.AppId)
                .WithTenantId(_config.TenantId)
                .WithClientSecret(_config.ClientSecret)
                .Build();
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Set up the Microsoft Graph service client with client credentials

            _graphClient = new GraphServiceClient(authProvider);
        }
      
        public override Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            throw new NotImplementedException();
        }
        public override Task<OperationResult> SignInUser(string email, string password, bool rememberMe)
        {
            throw new NotImplementedException();
        }

  
        public override  async Task<UserClaimsService> GetUserClaims(string emailLogin)
        {
            await Task.CompletedTask;

           
            if (string.IsNullOrEmpty(emailLogin))
            {
                //throw new NullReferenceException(nameof(emailClaim));
                IntLoggedUser = new UserClaimsService("",
                    UserRolesEnum.Anonymous ,
                    "",
                    "");
                return IntLoggedUser;
            }

            var userRoleClaim = _claims.FirstOrDefault(x => x.Type == "extension_userrole");
            UserRolesEnum userRole = UserRolesEnum.Anonymous ;
            if (userRoleClaim != null)
            {
                Enum.TryParse(userRoleClaim.Value, out userRole);

            }
  

            IntLoggedUser = new UserClaimsService(emailLogin,
                userRole,
                "",
                "");

             switch (userRole)
             {
                 case UserRolesEnum.Anonymous:
                     break;
                 case UserRolesEnum.Tenant:
                     IntLoggedUser = new UserClaimsService(emailLogin,
                         userRole,
                         _claims.FirstOrDefault(x => x.Type == "extension_PropertyCode")?.Value,
                         _claims.FirstOrDefault(x => x.Type == "extension_UnitNumber")?.Value);

                     break;
                 case UserRolesEnum.Superintendent:
                     IntLoggedUser = new UserClaimsService(emailLogin,
                         userRole,
                         _claims.FirstOrDefault(x => x.Type == "extension_PropertyCode")?.Value ,
                         "");

                     break;
                 case UserRolesEnum.Administrator:
                     break;
                 case UserRolesEnum.Worker:

                     break;
             }



            return IntLoggedUser;
        }

        public override string GetLoginFromClaimsPrinciple(ClaimsPrincipal principle)
        {
            _claims = principle.Claims;
            return principle.Claims.FirstOrDefault(x => x.Type == "emails")?.Value;
        }


        public override  async Task SetUserClaims(string email, UserRolesEnum userRole, string propCode, string unitNumber)
        {


            B2CCustomAttributeHelper helper = new B2CCustomAttributeHelper(_config.B2cExtensionAppClientId);
            string userRoleAttributeName = helper.GetCompleteAttributeName("userrole");
            string propCodeAttributeName = helper.GetCompleteAttributeName("PropertyCode");


            IDictionary<string, object> extensionInstance = new Dictionary<string, object>();

            extensionInstance.Add(helper.GetCompleteAttributeName("userrole"), userRole.ToString( ));
            if (!string.IsNullOrEmpty(propCode ))
                extensionInstance.Add(helper.GetCompleteAttributeName("PropertyCode"), propCode);
            if (!string.IsNullOrEmpty(unitNumber ))
                extensionInstance.Add(helper.GetCompleteAttributeName("UnitNumber"), unitNumber);

            var userToUpdate = new User
            {
                AdditionalData = extensionInstance
            };

            var result = await _graphClient.Users
                .Request()
                .Filter($"identities/any(c:c/issuerAssignedId eq '{IntLoggedUser.Login}' and c/issuer eq '{_config.TenantId}')")
                .Select(e => new
                {
                    e.DisplayName,
                    e.Id,
                    e.Identities
                })
                .GetAsync();

            var user = result.CurrentPage.FirstOrDefault();

            ObjectIdentity objIdentity = user?.Identities.FirstOrDefault();

            if (objIdentity != null && objIdentity.IssuerAssignedId == IntLoggedUser.Login)
            {
                await _graphClient.Users[user.Id]
                    .Request()
                    .UpdateAsync(userToUpdate);
            }
           

        }
     
 
       
    }
}
