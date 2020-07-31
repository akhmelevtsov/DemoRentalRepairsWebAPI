using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;

namespace Demo.RentalRepairs.Core.Services
{
    public class NotifyPartiesService : INotifyPartiesService
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateDataService _templateDataService;
        private readonly IPropertyRepository _propertyRepository;


        public NotifyPartiesService(IEmailService emailService, ITemplateDataService templateDataService,IPropertyRepository propertyRepository)
        {
            _emailService = emailService;
            _templateDataService = templateDataService;
            _propertyRepository = propertyRepository;
        }

        public EmailInfo CreateAndSendEmail(TenantRequest tenantRequest)
        {
          
            var email = new PropertyMessageFactory(_templateDataService,  tenantRequest, _propertyRepository ).CreateTenantRequestEmail();
            _emailService.SendEmail(email);
            return email;
        }
    }  
}
