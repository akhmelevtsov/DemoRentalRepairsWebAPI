using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class UserAuthorizationMockService : IUserAuthCoreService
    {
        public UserAuthorizationMockService()
        {
            AuthDomainService = new UserAuthDomainMockService();
        }

        public IUserAuthDomainService AuthDomainService { get; set; }

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress)
        {
           return new LoggedUser("");
        }

        public void SetUser(LoggedUser loggedUser)
        {
            
        }
    }
}
