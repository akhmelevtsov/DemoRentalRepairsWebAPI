using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Entities.Extensions;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Core.Services
{
    public class NotifyPartiesService : INotifyPartiesService
    {
        private readonly IEmailService _emailService;
        private readonly ITemplateDataService _templateDataService;

        public NotifyPartiesService(IEmailService emailService, ITemplateDataService templateDataService)
        {
            _emailService = emailService;
            _templateDataService = templateDataService;
        }

        public EmailInfo CreateAndSendEmail(TenantRequest tenantRequest)
        {
          
            var email = new PropertyMessageFactory(_templateDataService, tenantRequest).CreateTenantRequestEmail();
            _emailService.SendEmail(email);
            return email;
        }
    }  
}
