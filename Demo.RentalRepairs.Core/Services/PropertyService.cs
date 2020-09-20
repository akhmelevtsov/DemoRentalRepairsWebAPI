using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        //private readonly IUserClaimsService _claimService;

        private readonly DomainValidationService _validationService = new DomainValidationService();

       

        public PropertyService(IPropertyRepository propertyRepository, INotifyPartiesService notifyPartiesService, IUserAuthorizationService authorizationService)
        {

            _propertyRepository = propertyRepository;
            _notifyPartiesService = notifyPartiesService;
            _authService = authorizationService;
            //_claimService = authorizationService.LoggedUser;
        }

  
        public IEnumerable<Property> GetAllProperties()
        {
           
            GetClaims().Check(() => GetClaims().IsAnonymousUser()); 
            return _propertyRepository.GetAllProperties();
        }

 

        public Property GetPropertyByCode(string propCode)
        {
            _validationService.ValidatePropertyCode(propCode);
           
            GetClaims().Check(() => GetClaims().IsAnonymousUser() || GetClaims().IsAuthenticatedTenant() || GetClaims().IsRegisteredTenant(propCode)  || GetClaims().IsRegisteredSuperintendent( propCode ) );
            var prop = _propertyRepository.GetPropertyByCode(propCode);
            if (prop == null)
                throw new DomainEntityNotFoundException("property_not_found", $"property not found by code:{propCode}");
            return prop;
        }
        public async Task<Worker> RegisterWorkerAsync(PersonContactInfo workerInfo)
        {
            await Task.CompletedTask;

            //_authService.Check(() => _authService.IsRegisteredWorker());
            var worker = new Worker(workerInfo);
            _propertyRepository.AddWorker(worker);
            await _authService.SetUserClaims(GetClaims().Login , UserRolesEnum.Worker , null, null);
            return worker;
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
         
            GetClaims().Check(() => GetClaims().IsRegisteredSuperintendent( ));
            return _propertyRepository.GetAllWorkers();
        }
        public Worker GetWorkerByEmail(string email)
        {
            GetClaims().Check(() => GetClaims().IsRegisteredSuperintendent() || GetClaims().IsRegisteredWorker() );
            return _propertyRepository.GetWorkerByEmail(email);
        }


        public  async Task<Property> RegisterPropertyAsync(RegisterPropertyCommand propertyInfo)
        {
            await Task.CompletedTask;
            //_authService.Check(() => _authService.IsAuthenticatedSuperintendent());
            var prop = new Property(propertyInfo);
            var p = _propertyRepository.GetPropertyByCode(prop.Code);
            if (p!=null)
                throw new DomainEntityDuplicateException("duplicate_property","duplicate property name");
            _propertyRepository.AddProperty(prop);
            await _authService.SetUserClaims(GetClaims().Login, UserRolesEnum.Superintendent , prop.Code, null);
            return prop;

        }
        // tenants
        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
          
            GetClaims().Check(() => GetClaims().IsRegisteredSuperintendent(propertyCode));
            _validationService.ValidatePropertyCode(propertyCode);
            return _propertyRepository.GetPropertyTenants(propertyCode);
        }

        public async Task<Tenant> RegisterTenantAsync(string propertyCode, PersonContactInfo contactInfo, string unitNumber)
        {
            await Task.CompletedTask;
            //_authService.Check(() => _authService.IsAuthenticatedTenant());

            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(unitNumber);
            _validationService.Validate(contactInfo);

            var property = _propertyRepository.GetPropertyByCode(propertyCode);
            if (property == null)
                throw new DomainEntityNotFoundException("property_not_found", "property not found");
            var tenant =_propertyRepository.GetTenantByUnitNumber(unitNumber, propertyCode);
            if (tenant != null)
                throw new DomainEntityDuplicateException("duplicate_tenant", "duplicate tenant");
            tenant = property.RegisterTenant( contactInfo, unitNumber);
           
            _propertyRepository.AddTenant(tenant);
            await _authService.SetUserClaims(GetClaims().Login, UserRolesEnum.Tenant , propertyCode, unitNumber);
            return tenant;

        }

       

        public Tenant GetTenantByPropertyUnit(string propertyCode, string propertyUnit)
        {
            
            GetClaims().Check(() => GetClaims().IsRegisteredTenant(propertyCode , propertyUnit ) || GetClaims().IsRegisteredSuperintendent( propertyCode ));
            _validationService.ValidatePropertyCode(propertyCode);
            var tenant = _propertyRepository.GetTenantByUnitNumber(propertyUnit, propertyCode);

            return tenant;

        }

        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            GetClaims().Check(() => GetClaims().IsRegisteredWorker(workerEmail));
            return _propertyRepository.GetWorkerRequests(workerEmail);
        }
        //Requests

        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            _validationService.ValidatePropertyCode(propertyCode);
            GetClaims().Check(() => GetClaims().IsRegisteredSuperintendent(propertyCode));
            return _propertyRepository.GetPropertyRequests(propertyCode);

        }
        public IEnumerable<TenantRequest> GetTenantRequests(string propertyCode, string tenantUnit)
        {
            
            _validationService.ValidatePropertyCode(propertyCode);
            _validationService.ValidatePropertyUnit(tenantUnit);
            GetClaims().Check(() => GetClaims().IsRegisteredTenant(propertyCode, tenantUnit));
            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propertyCode);

            var retList = new List<TenantRequest>();

            if (tenant != null)
            {
               
                retList = _propertyRepository.GetTenantRequests(tenant.Id).ToList();
            }

            return retList;
        }
        public async Task<TenantRequest> RegisterTenantRequestAsync(string propCode, string tenantUnit,
            RegisterTenantRequestCommand tenantRequestDoc)
        {
           
            GetClaims().Check(() => GetClaims().IsRegisteredTenant( propCode, tenantUnit));
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);

            var tenant = _propertyRepository.GetTenantByUnitNumber(tenantUnit, propCode);
            if (tenant == null)
                return null;
            var tenantRequest = tenant.AddRequest( tenantRequestDoc);

            _propertyRepository.AddTenantRequest(tenantRequest);

            await _notifyPartiesService.CreateAndSendEmailAsync(tenantRequest);
        
            return tenantRequest;
        }

        public async  Task<TenantRequest> ExecuteTenantRequestCommandAsync(string propCode, string tenantUnit,
            string requestCode,
            ITenantRequestCommand command)
        {
           
            var tenantRequest = this.GetTenantRequest(propCode, tenantUnit, requestCode);

            GetClaims().Check(() => GetClaims().IsUserCommand(command.GetType()));
            tenantRequest = tenantRequest.ExecuteCommand( command);

            _propertyRepository.UpdateTenantRequest(tenantRequest);

            await _notifyPartiesService.CreateAndSendEmailAsync(tenantRequest);

            return tenantRequest;
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            _validationService.ValidatePropertyCode(propCode);
            _validationService.ValidatePropertyUnit(tenantUnit);
            GetClaims().Check(() => GetClaims().IsRegisteredTenant(propCode, tenantUnit)  
                                   || GetClaims().IsRegisteredSuperintendent( propCode )
                                   || GetClaims().IsRegisteredWorker( )
                );
            var tenantRequest = _propertyRepository.GetTenantRequest(propCode, tenantUnit, requestCode);
            if (tenantRequest == null)
                throw new DomainEntityNotFoundException("request_not_found", "request not found");
            return tenantRequest;
        }

        private IUserClaimsService GetClaims()
        {
            return _authService.LoggedUser;
        }
    }
}
