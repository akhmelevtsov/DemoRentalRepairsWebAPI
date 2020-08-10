using System;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities
{
    public class TenantRequestTbl
    {
        public Guid ID { get; set; }
        public TenantTbl Tenant { get; set; }
         public Guid TenantID { get; set; }
        public TenantRequestStatusEnum RequestStatus { get; set; }
     
        public int ServiceWorkOrderCount { get; set; }
        public string Code { get; set; }
       
        public DateTime DateCreated { get; set; }
        public string RequestChanges { get; set;  }
    }
}
