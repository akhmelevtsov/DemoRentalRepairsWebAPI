using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public class UserAuthCoreService : IUserAuthCoreService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserAuthDomainService _authDomainService;

        public UserAuthCoreService(IPropertyRepository propertyRepository, IUserAuthDomainService authDomainService)
        {
            _propertyRepository = propertyRepository;
            _authDomainService = authDomainService;
        }

        public IUserAuthDomainService AuthDomainService => _authDomainService;

        public LoggedUser SetUser(UserRolesEnum userRole, string emailAddress)
        {
            switch (userRole)
            {
                case UserRolesEnum.Superintendent:
                    var prop = _propertyRepository.FindPropertyByLoginEmail(emailAddress);
                    // if not found, property is not registered yet
                    _authDomainService.LoggedUser = prop == null ? new LoggedUser(emailAddress, UserRolesEnum.Superintendent) : new LoggedUser(emailAddress, UserRolesEnum.Superintendent, propCode: prop.Code);
                    break;
                case UserRolesEnum.Tenant:
                    var tenant = _propertyRepository.FindTenantByLoginEmail(emailAddress);
                    // if not found, tenant is not registered yet
                    _authDomainService.LoggedUser = tenant == null ? new LoggedUser(emailAddress, UserRolesEnum.Tenant) : new LoggedUser(emailAddress, UserRolesEnum.Tenant, propCode: tenant.PropertyCode, unitNumber: tenant.UnitNumber);
                    break;
                case UserRolesEnum.Worker:
                    var worker = _propertyRepository.FindWorkerByLoginEmail(emailAddress);
                    // if not found, worker is not registered yet
                    _authDomainService.LoggedUser = worker == null
                        ? new LoggedUser(emailAddress, UserRolesEnum.Worker)
                        : new LoggedUser(emailAddress, UserRolesEnum.Worker) { };
                    break;

                default:
                    _authDomainService.LoggedUser = new LoggedUser(emailAddress, userRole);
                    break;
            }

            return _authDomainService.LoggedUser;
        }

        public void SetUser(LoggedUser loggedUser)
        {
            _authDomainService.LoggedUser = loggedUser;
        }
    }
}
