using System;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Exceptions;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ISecurityService _securityService;
        private readonly DomainValidationService _validationService = new DomainValidationService();

        private UserClaims _loggedUser;
        public UserAuthorizationService(IPropertyRepository propertyRepository, ISecurityService securityService)
        {
            _propertyRepository = propertyRepository;
            _securityService = securityService;
        }

        public UserClaims LoggedUser => _loggedUser;


 
        public void SetUser(UserClaims loggedUser)
        {
            _loggedUser = loggedUser;
        }

        public async Task<OperationResult> RegisterUser(UserRolesEnum userRole, string email, string password)
        {
            return await _securityService.RegisterUser(email, password);
        }

        public async Task<UserClaims> GetUserClaims(string email)
        {
            _loggedUser = await  _securityService.GetLoggedUserClaims(email);
         
            return _loggedUser;
        }

        public void Check(Func<bool> func)
        {
            if (func()) return;

            throw new CoreAuthorizationException("access_denied", "access denied");
        }

        public bool IsAuthenticatedTenant()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty(_loggedUser.Login)
                                                             && string.IsNullOrEmpty(_loggedUser.PropCode)
                                                             && string.IsNullOrEmpty(_loggedUser.UnitNumber))
            {
                return true;
            }

            return false;
        }
        public bool IsRegisteredTenant(string propCode, string tenantUnit = null)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Tenant && !string.IsNullOrEmpty( _loggedUser.Login)
                                                             && !string.IsNullOrEmpty(_loggedUser.PropCode)
                                                             && !string.IsNullOrEmpty(_loggedUser.UnitNumber))
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

        public bool IsAuthenticatedSuperintendent()
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent && !string.IsNullOrEmpty(_loggedUser.Login)
                                                                     && string.IsNullOrEmpty(_loggedUser.PropCode))
            {
                    return true;
            }

            return false;
        }

        public bool IsAnonymousUser()
        {
            return (_loggedUser.UserRole == UserRolesEnum.Anonymous && !string.IsNullOrEmpty(_loggedUser.Login));
        }

        public async Task SetUserClaims(UserRolesEnum userRole, string propCode, string userNumber)
        {
            await _securityService.SetLoggedUserClaims(_loggedUser.Login, userRole, propCode, userNumber);
        }

        public bool IsRegisteredSuperintendent(string propCode=null)
        {
            if (_loggedUser.UserRole == UserRolesEnum.Superintendent  && !string.IsNullOrEmpty(_loggedUser.Login)
                                                                      && !string.IsNullOrEmpty(_loggedUser.PropCode))
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

       

        public UserClaims SetUser(UserRolesEnum userRole, string emailAddress, string propertyCode = null, string unitNumber = null)
        {
            
          
            _loggedUser = new UserClaims(emailAddress, userRole, propertyCode , unitNumber ); ;
            return _loggedUser;
        }
    }
}
