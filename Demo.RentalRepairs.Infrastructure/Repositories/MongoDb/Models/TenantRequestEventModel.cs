using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models
{
    public class TenantRequestEventModel
    {
        public TenantRequestStatusEnum TenantRequestStatus { get; set; }
        public ITenantRequestCommand Command { get; set; }
        public int Num { get; set; }
    }
}
