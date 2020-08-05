﻿using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Identity.AspNetCore.Data;
using Microsoft.AspNetCore.Identity;

namespace Demo.RentalRepairs.Infrastructure.Identity.AspNetCore
{
    public class SecuritySignInService : ISecuritySignInService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SecuritySignInService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task SignInAsync(object user, bool isPersistent)
        {
            await _signInManager.SignInAsync((ApplicationUser)user, isPersistent: false);
        }

    
    }
}
