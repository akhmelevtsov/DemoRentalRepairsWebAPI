using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity
{
    public class TenantTbl : Microsoft.Azure.Cosmos.Table.TableEntity
    {

        public TenantTbl(string propertyCode, string unitNumber)
        {
            this.PartitionKey = propertyCode;
            this.RowKey = unitNumber;
        }

        public TenantTbl()
        {
            
        }
        public Guid ID { get; set; }
        public string ContactInfo { get; set; }

        public string PropertyCode
        {
            get => PartitionKey;
   
        }  //Code

        public string UnitNumber => RowKey;
        public DateTime DateCreated { get; set; }

    }
}
