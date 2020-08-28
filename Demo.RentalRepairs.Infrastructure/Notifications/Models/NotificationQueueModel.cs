using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models;

namespace Demo.RentalRepairs.Infrastructure.Notifications.Models
{
    public class NotificationQueueModel
    {
        public TenantRequestModel TenantRequestModel { get; set; }
        public WorkerModel WorkerModel { get; set; }
    }
}
