using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;

namespace Demo.RentalRepairs.Core.Services
{
    public class NotifyPartiesService : BaseNotifyPartiesService , INotifyPartiesService
    {
        private readonly IEmailBuilderService _emailBuilderService;
        private readonly IEmailService _emailService;
       


        public NotifyPartiesService(IEmailBuilderService emailBuilderService, IEmailService emailService, IPropertyRepository propertyRepository) :base (propertyRepository )
        {
            _emailBuilderService = emailBuilderService;
            _emailService = emailService;
           
        }

        public async Task CreateAndSendEmailAsync(TenantRequest tenantRequest)
        {
            var worker = GetWorkerDetails(tenantRequest);

            await CreateAndSendEmailAsync(tenantRequest, worker);
        }

        public async Task CreateAndSendEmailAsync(TenantRequest tenantRequest, Worker worker)
        {
            await Task.CompletedTask;
            var email = _emailBuilderService.CreateTenantRequestEmail(tenantRequest , worker );
            if (email != null)
             await _emailService.SendEmailAsync(email);
        }

       
    }  
}
