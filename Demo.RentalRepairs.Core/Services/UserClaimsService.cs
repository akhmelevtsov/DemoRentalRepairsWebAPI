using System;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserClaimsService : IUserClaimsService
    {
        private readonly DomainValidationService _validationService = new DomainValidationService();

        public UserClaimsService(string emailAddress, UserRolesEnum userRole = UserRolesEnum.Anonymous ,  string propCode = "", string unitNumber = "")
        {
            Login = emailAddress;
            UserRole = userRole;
            PropCode = propCode;
            UnitNumber = unitNumber;
        }
        public string Login { get; set; }
        public string PropCode { get; private set; }
        public string UnitNumber { get; private set; }
        public  UserRolesEnum UserRole { get; private set;  }

        public bool IsRegisteredTenant()
        {
            return UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty(PropCode) &&
                   !string.IsNullOrEmpty(UnitNumber);
        }
        public void Check(Func<bool> func)
        {
            if (func()) return;

            throw new CoreAuthorizationException("access_denied", "access denied");
        }

        public bool IsAuthenticatedTenant()
        {
            if (UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty(Login)
                                                             && string.IsNullOrEmpty(PropCode)
                                                             && string.IsNullOrEmpty(UnitNumber))
            {
                return true;
            }

            return false;
        }
        public bool IsRegisteredTenant(string propCode, string tenantUnit = null)
        {
            if (UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty(Login)
                                                             && !string.IsNullOrEmpty(PropCode)
                                                             && !string.IsNullOrEmpty(UnitNumber))
            {
                _validationService.ValidatePropertyCode(propCode);
                if (tenantUnit != null)
                    _validationService.ValidatePropertyUnit(tenantUnit);

                if (PropCode == propCode)
                {

                    if (tenantUnit == null)
                        return true;
                    if (UnitNumber == tenantUnit)
                        return true;
                }

            }

            return false;
        }

        public bool IsAuthenticatedSuperintendent()
        {
            if (UserRole == UserRolesEnum.Superintendent && !string.IsNullOrEmpty(Login)
                                                                     && string.IsNullOrEmpty(PropCode))
            {
                return true;
            }

            return false;
        }

        public bool IsAnonymousUser()
        {
            return (UserRole == UserRolesEnum.Anonymous && !string.IsNullOrEmpty(Login));
        }

        public bool IsRegisteredSuperintendent(string propCode = null)
        {
            if (UserRole == UserRolesEnum.Superintendent && !string.IsNullOrEmpty(Login)
                                                                      && !string.IsNullOrEmpty(PropCode))
            {
                if (propCode != null)
                    _validationService.ValidatePropertyCode(propCode);
                else
                {
                    return true;
                }

                if (PropCode == propCode)
                {
                    return true;
                }

            }

            return false;
        }

        public bool IsRegisteredWorker(string email = null)
        {
            if (UserRole == UserRolesEnum.Worker && !string.IsNullOrEmpty(Login))
            {
                if (email == null)
                    return true;
                if (Login == email)
                    return true;

            }

            return false;
        }

        public bool IsUserCommand(Type type)
        {
            if (type == typeof(RegisterTenantRequestCommand))
            {
                if (UserRole == UserRolesEnum.Tenant) return true;
            }

            if (type == typeof(RejectTenantRequestCommand))
            {

                if (UserRole == UserRolesEnum.Superintendent) return true;
            }

            if (type == typeof(ScheduleServiceWorkCommand))
            {
                if (UserRole == UserRolesEnum.Superintendent) return true;
            }

            if (type == typeof(ReportServiceWorkCommand))
            {
                if (UserRole == UserRolesEnum.Worker) return true;
            }

            if (type == typeof(CloseTenantRequestCommand))
            {
                if (UserRole == UserRolesEnum.Superintendent) return true;
            }

            return false;
        }
    }
}
 