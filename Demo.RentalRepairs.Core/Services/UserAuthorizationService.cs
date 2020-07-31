using System;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly DomainValidationService _validationService = new DomainValidationService();

        private LoggedUser _loggedUser;
        public UserAuthorizationService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;

        }

        public LoggedUser LoggedUser => _loggedUser;


 
        public void SetUser(LoggedUser loggedUser)
        {
            _loggedUser = loggedUser;
        }
 
        public void Check(Func<bool> func)
        {
            if (func()) return;

            throw new CoreAuthorizationException("access_denied", "access denied");
        }

        public bool IsRegisteredTenant(string propCode, string tenantUnit = null)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty( _loggedUser.Login) )
            {
                _validationService.ValidatePropertyCode(propCode);
                if (tenantUnit != null)
                  _validationService.ValidatePropertyUnit(tenantUnit);

                if (_loggedUser.PropCode == propCode)
                {

                    if (tenantUnit == null)
                        return true;
                    if (_loggedUser.UnitNumber == tenantUnit)
                        return true;
                }

            }

            return false;
        }

        public bool IsLoggedTenant()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty(_loggedUser.Login))
            {
                return true;

            }

            return false;
        }

        public bool IsRegisteredSuperintendent(string propCode=null)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent  && !string.IsNullOrEmpty(_loggedUser.Login))
            {
                if (propCode != null)
                   _validationService.ValidatePropertyCode(propCode);
                else
                {
                    return true;
                } 

                if (_loggedUser.PropCode == propCode )
                {
                    return true;
                }

            }

            return false;
        }

        public bool IsRegisteredWorker(string email=null)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Worker  && !string.IsNullOrEmpty(_loggedUser.Login))
            {
                if (email == null)
                  return true;
                if (_loggedUser.Login == email)
                    return true;

            }

            return false;
        }

        public bool IsUserCommand(Type type)
        {
            if (type == typeof(RegisterTenantRequestCommand))
            {
                if (_loggedUser.UserRole == UserRolesEnum.Tenant) return true;
            }

            if (type == typeof(RejectTenantRequestCommand))
            {

                if (_loggedUser.UserRole == UserRolesEnum.Superintendent ) return true;
            }

            if (type == typeof(ScheduleServiceWorkCommand))
            {
                if (_loggedUser.UserRole == UserRolesEnum.Superintendent) return true;
            }

            if (type == typeof(ReportServiceWorkCommand) )
            {
                if (_loggedUser.UserRole == UserRolesEnum.Worker) return true;
            }
           
            if (type == typeof(CloseTenantRequestCommand))
            {
                if (_loggedUser.UserRole == UserRolesEnum.Superintendent) return true;
            }

            return false;
        }

        public void UserCanChangeTenantRequestStatus(TenantRequestStatusEnum newStatus)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent)
            {
                if (newStatus == TenantRequestStatusEnum.Scheduled ||
                    newStatus == TenantRequestStatusEnum.Declined || newStatus == TenantRequestStatusEnum.Closed)
                    return;

                throw new DomainAuthorizationException("super_cannot_change_request_status", "Superintendent has no permissions to change request status");

            }
            else if (_loggedUser.UserRole == UserRolesEnum.Worker)
            {

                if (newStatus == TenantRequestStatusEnum.Done ||
                    newStatus == TenantRequestStatusEnum.Failed)
                    return;

                throw new DomainAuthorizationException("worker_cannot_change_request_status", "Worker has no permissions to change request status");

            }
            else
            {
                throw new DomainAuthorizationException("user_not_allowed_to_change_request_status", "No permissions to change request status");
            }

        }
      

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode = null, string unitNumber = null)
        {
            
          
            _loggedUser = new LoggedUser(emailAddress, userRole, propertyCode , unitNumber ); ;
            return _loggedUser;
        }
    }
}
