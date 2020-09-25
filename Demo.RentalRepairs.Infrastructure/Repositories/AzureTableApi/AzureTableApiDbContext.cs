using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Entities;
using Microsoft.Azure.Cosmos.Table;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi
{
    public class AzureTableApiDbContext
    {
        private readonly CloudTableClient _tableClient;


        public AzureTableApiDbContext(RentalRepairsAzureTableApiDbSettings settings)
        {
           
            var storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
        }

        public const string PropertyTableName = "Properties";
        public const string TenantTableName = "Tenants";
        public const string RequestTableName = "Requests";
        public const string WorkerTableName = "Workers";
        public const string WorkerRequestTableName = "WorkerRequests";

        public CloudTable PropertyCloudTable => GetCloudTable(PropertyTableName);
        public CloudTable TenantCloudTable => GetCloudTable(TenantTableName);
        public CloudTable RequestCloudTable => GetCloudTable(RequestTableName);
        public CloudTable WorkerCloudTable => GetCloudTable(WorkerTableName);
        public CloudTable WorkerRequestsCloudTable => GetCloudTable(WorkerRequestTableName);


        private CloudTable GetCloudTable(string tableName)
        {
            CloudTable table = _tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        public void DropTables()
        {
            foreach (var table in _tableClient.ListTables())
                table.DeleteIfExists();
        }
        public void AddWorker(Worker worker)
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(new EntityMapper().CopyFrom(worker));
            var tbl = WorkerCloudTable;
            tbl.Execute(insertOrMergeOperation);

        }
    }
}
