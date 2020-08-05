using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class SecuritySignInMockService :ISecuritySignInService
    {
        public async Task SignInAsync(object user, bool isPersistent)
        {
            await Task.CompletedTask;

        }
    }
}
