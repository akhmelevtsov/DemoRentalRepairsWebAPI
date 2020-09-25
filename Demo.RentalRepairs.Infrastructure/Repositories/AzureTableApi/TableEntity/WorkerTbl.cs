using System;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity
{
    public class WorkerTbl : Microsoft.Azure.Cosmos.Table.TableEntity
    {
        public const string PartitionKeyConst = "Worker";
        public WorkerTbl(string email)
        {
            PartitionKey = PartitionKeyConst;
            RowKey = email;
        }

        public WorkerTbl()
        {
            
        }
        public Guid ID { get; set;  }
        public string ContactInfo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
