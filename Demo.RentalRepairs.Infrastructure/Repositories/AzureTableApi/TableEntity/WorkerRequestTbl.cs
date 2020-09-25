using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity
{
    public class WorkerRequestTbl : Microsoft.Azure.Cosmos.Table.TableEntity
    {
        public WorkerRequestTbl(string workerEmail, Guid requestGuid)
        {
            PartitionKey = workerEmail;
            RowKey = requestGuid.ToString();

        }

        public WorkerRequestTbl()
        {

        }

        public string TenantEmail { get; set; }
        public string WorkerEmail => PartitionKey;
        public Guid RequestId => Guid.Parse(RowKey);
    }
}
