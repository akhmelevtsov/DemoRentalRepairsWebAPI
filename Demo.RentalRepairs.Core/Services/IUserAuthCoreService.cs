using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public interface IUserAuthCoreService
    {
        IUserAuthDomainService AuthDomainService { get; }
        LoggedUser SetUser(UserRolesEnum userRole, string emailAddress);
        void SetUser(LoggedUser loggedUser);

    }
}