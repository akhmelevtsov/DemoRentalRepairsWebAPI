using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.RentalRepairs.Core.Security;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core.Services
{
    public class CoreSecurityService
    {

        private readonly Dictionary<PolicyEnum, Dictionary<ClaimEnum, string>> _policies =
            new Dictionary<PolicyEnum, Dictionary<ClaimEnum, string>>()
            {
                {
                    PolicyEnum.RegisteredTenant, new Dictionary<ClaimEnum, string>()
                    {
                        {ClaimEnum.UserRole, UserRolesEnum.Tenant.ToString()},
                        {ClaimEnum.PropertyCode, ""},
                        {ClaimEnum.UnitNumber, ""}

                    }
                },
                {
                    PolicyEnum.RegisteredSuperintendent, new Dictionary<ClaimEnum, string>()
                    {
                        {ClaimEnum.UserRole, UserRolesEnum.Superintendent.ToString()},
                        {ClaimEnum.PropertyCode, ""}


                    }
                },
                {
                    PolicyEnum.RegisteredWorker, new Dictionary<ClaimEnum, string>()
                    {
                        {ClaimEnum.UserRole, UserRolesEnum.Worker.ToString()}

                    }
                },
                {
                    PolicyEnum.RegisteredAdministrator, new Dictionary<ClaimEnum, string>()
                    {
                        {ClaimEnum.UserRole, UserRolesEnum.Administrator.ToString()}

                    }
                },
            };


        //public void CheckClaims(LoggedUser loggedUser, List<PolicyEnum> policies,
        //    Dictionary<ClaimEnum, string> claimParams = null)
        //{
        //    if (loggedUser == null)
        //        throw new ArgumentNullException(nameof(loggedUser));
        //    var userClaims = new Dictionary<ClaimEnum, string> {{ClaimEnum.UserRole, loggedUser.UserRole.ToString()}};

        //    if (!string.IsNullOrEmpty( loggedUser.PropCode))
        //        userClaims.Add(ClaimEnum.PropertyCode, loggedUser.PropCode);
        //    if (!string.IsNullOrEmpty(loggedUser.UnitNumber ))
        //        userClaims.Add(ClaimEnum.PropertyCode, loggedUser.UnitNumber);
        //    CheckClaims(userClaims, policies, claimParams);
        //}

        public void CheckClaims(Dictionary<ClaimEnum, string> userClaims, List<(PolicyEnum policy, Dictionary<ClaimEnum, string> claimParams)> policies        )
        {
            if (userClaims == null || !userClaims.Any())
                throw new ArgumentNullException(nameof(userClaims));
            if (policies == null || !policies.Any())
                throw new ArgumentNullException(nameof(policies));

            foreach (var p in policies)
            {
                var policyClaims = _policies[p.policy];
                if (this.ParamMatch(policyClaims, userClaims, p.claimParams ))
                    return;
            }

            throw new AccessViolationException();


        }


        private bool ParamMatch(Dictionary<ClaimEnum, string> policyClaims, IReadOnlyDictionary<ClaimEnum, string> userClaims, IReadOnlyDictionary<ClaimEnum, string> claimParams)
        {
            if (policyClaims.Count != userClaims.Count)
                return false;

            foreach (var p in policyClaims)
            {
                if (!userClaims.ContainsKey(p.Key))
                    return false;
                var userClaimValue = userClaims[p.Key];
                if (string.IsNullOrEmpty(p.Value)) // compare parameter
                {
                    if (claimParams == null || !claimParams.ContainsKey(p.Key))
                        return false;
                    var claimParamsVal = claimParams[p.Key];
                    if (userClaimValue != claimParamsVal)
                        return false;
                }
                else
                {
                    if (p.Value != userClaimValue)
                        return false;
                }

            }

            return true;
        }



    }
}
