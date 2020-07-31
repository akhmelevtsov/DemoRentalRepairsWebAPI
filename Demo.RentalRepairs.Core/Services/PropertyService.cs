using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;
using Demo.RentalRepairs.Domain.Services;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly INotifyPartiesService _notifyPartiesService;
        private readonly IUserAuthorizationService _authService;
      

        private readonly DomainValidationService _validationService = new DomainValidationService();

       

        public PropertyService(IPropertyRepository propertyRepository, INotifyPartiesService notifyPartiesService, IUserAuthorizationService authorizationService)
        {

            _propertyRepository = propertyRepository;
            _notifyPartiesService = notifyPartiesService;
            _authService = authorizationService;
        }

  
        public IEnumerable<Property> GetAllProperties()
        {
            //_authService.UserCanGetListOfProperties();
            _authService.Check(() => _authService.IsLoggedTenant()); 
            return _propertyRepository.GetAllProperties();
        }

 

        public Property GetPropertyByCode(string propCode)
        {
            _validationService.ValidatePropertyCode(propCode);
            //_authService.UserCanGetPropertyDetails(propCode);
            _authService.Check(() => _authService.IsLoggedTenant() || _authService.IsRegisteredTenant(propCode)  || _authService.IsRegisteredSuperintendent( propCode ) );
            var prop = _propertyRepository.GetPropertyByCode(propCode);

            return prop;
        }
        public Worker AddWorker(PersonContactInfo workerInfo)
        {
            //_authService.UserCanRegisterWorker();
            _authService.Check(() => _authService.IsRegisteredWorker());
            var worker = new Worker(workerInfo);
            _propertyRepository.AddWorker(worker);
            return worker;
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            //_authService.UserCanGetListOfAllWorkers();
            _authService.Check(() => _authService.IsRegisteredSuperintendent( ));
            return _propertyRepository.GetAllWorkers();
        }
        public Worker GetWorkerByEmail(string email)
        {
            _authService.Check(() => _authService.IsRegisteredSuperintendent() || _authService.IsRegisteredWorker() );
            return _propertyRepository.GetWorkerByEmail(email);
        }


        public Property AddProperty(AddPropertyCommand propertyInfo)
        {
            //_authService.UserCanRegisterProperty();
            _authService.Check(() => _authService.IsRegisteredSuperintendent());
            var prop = new Property(propertyInfo)
            {
                LoginEmail = _authService.LoggedUser.Login
            };
            _propertyRepository.AddProperty(prop);
            return prop;

        }
        // tenants
        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            //_authService.UserCanGetListOfPropertyTenants(propertyCode);
            _authService.Check(() => _authService.IsRegisteredSuperintendent(propertyCode));
            _validationService.ValidatePropertyCode(propertyCode);
            return _propertyRepository.GetPropertyTenants(propertyCode);
        }

        public Tenant AddTenant(string propertyCode, PersonContactInfo contactInfo, string unitNumber)
        {
            //_authService.UserCanRegisterTenant();
            _authService.Check(() => _authService.IsLoggedTenant());

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(unitNumber);
            _validationService.Validate(contactInfo);

            var property = _propertyRepository.GetPropertyByCode(propertyCode);
            var tenant = property.RegisterTenant( contactInfo, unitNumber);
            tenant.LoginEmail = _authService.LoggedUser.Login;
            _propertyRepository.AddTenant(tenant);
            
            return tenant;

        }

       

        public Tenant GetTenantByPropertyUnit(string propertyCode, string propertyUnit)
        {
            //_authService.UserCanGetTenantDetails(propertyCode, propertyUnit);
            _authService.Check(() => _authService.IsRegisteredTenant(propertyCode , propertyUnit ) || _authService.IsRegisteredSuperintendent( propertyCode ));
            _validationService.ValidatePropertyCode(propertyCode);
            var tenant = _propertyRepository.GetTenantByUnitNumber(propertyUnit, propertyCode);

            return tenant;

        }

        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            _authService.Check(() => _authService.IsRegisteredWorker(workerEmail));
            return _propertyRepository.GetWorkerRequests(workerEmail);
        }
        //Requests

        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            _validationService.ValidatePropertyCode(propertyCode);
            _authService.Check(() => _authService.IsRegisteredSuperintendent(propertyCode));
            return _propertyRepository.GetPropertyRequests(propertyCode);

        }
        public IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit)
        {
            //_authService.UserCanGetListOfTenantRequests();
            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(tenantUnit);
            _authService.Check(() => _authService.IsRegisteredTenant(propertyCode, tenantUnit));
            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propertyCode);

            var retList = new List<TenantRequest>();

            if (tenant != null)
            {
                //_authService.UserCanGetListOfTenantRequests(propertyCode, tenantUnit);
                retList = _propertyRepository.GetTenantRequests(tenant.Id).ToList();
            }

            return retList;
        }
        public TenantRequest RegisterTenantRequest(string propCode, string tenantUnit, RegisterTenantRequestCommand tenantRequestDoc)
        {
            //_authService.UserCanRegisterTenantRequest(propCode, tenantUnit);
            _authService.Check(() => _authService.IsRegisteredTenant( propCode, tenantUnit));
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propCode);
            if (tenant == null)
                return null;
            var tenantRequest = tenant.AddRequest( tenantRequestDoc);

            _propertyRepository.AddTenantRequest(tenantRequest);

            _notifyPartiesService.CreateAndSendEmail(tenantRequest);
        
            return tenantRequest;
        }

        public TenantRequest ExecuteTenantRequestCommand(string propCode, string tenantUnit, string requestCode,
            ITenantRequestCommand command)
        {
            //_authService.UserCanChangeTenantRequestStatus(newStatus);

           // _authService.Check(() => _authService.IsRegisteredTenant(propCode, tenantUnit));
            var tenantRequest = GetTenantRequest(propCode, tenantUnit, requestCode);

            _authService.Check(() => _authService.IsUserCommand(command.GetType()));
            tenantRequest = tenantRequest.ExecuteCommand( command);

            _propertyRepository.UpdateTenantRequest(tenantRequest);

            _notifyPartiesService.CreateAndSendEmail(tenantRequest);

            return tenantRequest;
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);
            _authService.Check(() => _authService.IsRegisteredTenant(propCode, tenantUnit)  
                                   || _authService.IsRegisteredSuperintendent( propCode )
                                   || _authService.IsRegisteredWorker( )
                );
            var tenantRequest = _propertyRepository.GetTenantRequest(propCode, tenantUnit, requestCode);
            if (tenantRequest == null)
                throw new DomainEntityNotFoundException("request_not_found", "request not found");
            return tenantRequest;
        }
      

    }
}
