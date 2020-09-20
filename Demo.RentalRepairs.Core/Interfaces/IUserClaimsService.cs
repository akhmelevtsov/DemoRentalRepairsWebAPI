using System;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface IUserClaimsService
    {
        string Login { get; set; }
        string PropCode { get; }
        string UnitNumber { get; }
        UserRolesEnum UserRole { get; }
        bool IsRegisteredTenant();
        void Check(Func<bool> action);
        bool IsRegisteredTenant(string propCode, string tenantUnit = null);
        bool IsAuthenticatedTenant();
        bool IsRegisteredSuperintendent(string propCode = null);
        bool IsRegisteredWorker(string email = null);
        bool IsUserCommand(Type getType);
        //bool IsAuthenticatedSuperintendent();
        bool IsAnonymousUser();
    }
}