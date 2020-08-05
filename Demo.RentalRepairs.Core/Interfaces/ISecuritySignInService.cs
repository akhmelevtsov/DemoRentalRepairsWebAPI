using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface ISecuritySignInService
    {
        Task SignInAsync(object user, bool isPersistent);
      
    }
}
