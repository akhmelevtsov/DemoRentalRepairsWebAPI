using System;
using Demo.RentalRepairs.Domain.Enums;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity
{
    public class TenantRequestTbl : Microsoft.Azure.Cosmos.Table.TableEntity
    {
        public TenantRequestTbl(string tenantEmail, string code)
        {
            PartitionKey = tenantEmail;
            RowKey = code;

        }

        public TenantRequestTbl()
        {
            
        }
        public Guid ID { get; set; }
        
        public Guid TenantID { get; set; }
        public string TenantEmail => PartitionKey;
        public string RequestStatus { get; set; }
     
        public int ServiceWorkOrderCount { get; set; }
        public string Code => RowKey;
        public DateTime DateCreated { get; set; }
        public string RequestChanges { get; set;  }
        public string PropertyCode { get; set; }
        public string UnitNumber { get; set; }
    }
}
