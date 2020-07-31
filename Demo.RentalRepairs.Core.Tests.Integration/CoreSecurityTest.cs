using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Security;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.RentalRepairs.Core.Tests.Integration
{
    [TestClass]
    public class CoreSecurityTest
    {
        [TestMethod]
        public void Test()
        {
            var s = new CoreSecurityService();
            var userClaims = new Dictionary<ClaimEnum, string>()
            {
                { ClaimEnum.UserRole, UserRolesEnum.Tenant.ToString()},
                { ClaimEnum.PropertyCode, "ppp" },
                { ClaimEnum.UnitNumber,  "123"}

            };

            s.CheckClaims(userClaims, 
            new List<(PolicyEnum policy, Dictionary<ClaimEnum, string> claimParams)>()
            {
                (PolicyEnum.RegisteredTenant ,new Dictionary<ClaimEnum, string>()
                    {
                        {ClaimEnum.UserRole, UserRolesEnum.Tenant.ToString( ) },
                        {ClaimEnum.PropertyCode, "pp"},
                        {ClaimEnum.UnitNumber, "123"  }
                    }
                )

            });

        }
    }
}
