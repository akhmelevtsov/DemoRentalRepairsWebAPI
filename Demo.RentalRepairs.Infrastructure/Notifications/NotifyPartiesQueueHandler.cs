using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Shared;

namespace Demo.RentalRepairs.Infrastructure.Notifications
{
    public class NotifyPartiesQueueHandler
    {
        private readonly IEmailBuilderService _emailBuilderService;


        public NotifyPartiesQueueHandler(IEmailBuilderService emailBuilderService)
        {
            _emailBuilderService = emailBuilderService;
        }
        public async Task<EmailInfo> Handle(string message, bool decode = true)
        {
            await Task.CompletedTask;
            var serializer = new ModelSerializer();
            var modelMapper = new ModelMapper();
            message = decode ? message.Base64Decode() : message;
            var n = serializer.Deserialize(message);
            var e = _emailBuilderService.CreateTenantRequestEmail(modelMapper.CopyFrom(n.TenantRequestModel), n.WorkerModel == null ? null : modelMapper.CopyFrom(n.WorkerModel));
            return e;

        }
    }
}
