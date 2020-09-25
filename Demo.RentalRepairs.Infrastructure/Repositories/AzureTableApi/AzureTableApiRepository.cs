using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi
{ 
    public class AzureTableApiRepository : IPropertyRepository
    {
        private readonly AzureTableApiDbContext _dbContext;
        private readonly EntityMapper _entityMapper = new EntityMapper();
        public AzureTableApiRepository(AzureTableApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddWorker(Worker worker)
        {
            TableOperation operation = TableOperation.InsertOrReplace(_entityMapper.CopyFrom(worker));
            var tbl = _dbContext.WorkerCloudTable;
            tbl.Execute(operation);
           
        }

        public void AddProperty(Property prop)
        {
            TableOperation operation = TableOperation.InsertOrReplace(_entityMapper.CopyFrom(prop));
            var tbl = _dbContext.PropertyCloudTable;
            tbl.Execute(operation);
        }

        public void AddTenant(Tenant tenant)
        {
            TableOperation operation = TableOperation.InsertOrReplace(_entityMapper.CopyFrom(tenant));
            var tbl = _dbContext.TenantCloudTable ;
            tbl.Execute(operation);
        }

        public void AddTenantRequest(TenantRequest tTenantRequest)
        {
            TableOperation operation = TableOperation.InsertOrReplace(_entityMapper.CopyFrom(tTenantRequest));
            var tbl = _dbContext.RequestCloudTable;
            tbl.Execute(operation);
        }

        public void UpdateTenantRequest(TenantRequest tTenantRequest)
        {
           
            var tbl = _entityMapper.CopyFrom(tTenantRequest);
            tbl.ETag = "*";
            TableOperation operation = TableOperation.Merge(tbl);
        
            _dbContext.RequestCloudTable.Execute(operation);

            if (tTenantRequest.RequestStatus == TenantRequestStatusEnum.Scheduled)
            {
                var change = tTenantRequest.RequestChanges.LastOrDefault(x =>
                    x.TenantRequestStatus == TenantRequestStatusEnum.Scheduled);
                if (change?.Command != null)
                {
                    var command = change.Command as ScheduleServiceWorkCommand;

                    operation = TableOperation.InsertOrReplace(new WorkerRequestTbl(command.WorkerEmailAddress, tTenantRequest.Id) { TenantEmail = tTenantRequest.TenantEmail});
                    _dbContext.WorkerRequestsCloudTable.Execute(operation);

                }
            }


        }

        private void AddWorkerOrder(ScheduleServiceWorkCommand command, TenantRequest tTenantRequest)
        {
            //WorkerRequestsCloudTable
  
        }

        public IEnumerable<Property> GetAllProperties()
        {
            IQueryable<PropertyTbl> linqQuery = _dbContext.PropertyCloudTable .CreateQuery<PropertyTbl>()
                .Where(x => x.PartitionKey == PropertyTbl.PartitionKeyConst)
                .Select(x => x);
            var list = linqQuery.ToList();
            return list.Select(x => _entityMapper.CopyFrom(x)).ToList();
        }

        public Property GetPropertyByCode(string propertyCode)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PropertyTbl>(PropertyTbl.PartitionKeyConst , propertyCode);
            var table = _dbContext.PropertyCloudTable;

            TableResult result = table.Execute(retrieveOperation);
            var  prop = result.Result as PropertyTbl ;
           

            return prop == null? null : _entityMapper.CopyFrom( prop );
        }

        public IEnumerable<Tenant> GetPropertyTenants(string propertyCode)
        {
            IQueryable<TenantTbl > linqQuery = _dbContext.TenantCloudTable.CreateQuery<TenantTbl>()
                .Where(x => x.PartitionKey == propertyCode)
                .Select(x => x);
            var list = linqQuery.ToList();

            return list.Select(x => _entityMapper.CopyFrom(x)).ToList();  // just tenant objects
        }

        public IEnumerable<TenantRequest> GetPropertyRequests(string propertyCode)
        {
            IQueryable<TenantRequestTbl> linqQuery = _dbContext.RequestCloudTable.CreateQuery<TenantRequestTbl>()
                .Where(x => x.PropertyCode == propertyCode)
                .Select(x => x);
            var list = linqQuery.ToList();
            return list.Select(x => _entityMapper.CopyFrom(x, GetTenantByUnitNumber(x.UnitNumber,x.PropertyCode  ))).ToList();


        }

        public Tenant GetTenantByUnitNumber(string tenantCode, string propCode)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TenantTbl>(propCode , tenantCode);
            var table = _dbContext.TenantCloudTable ;

            TableResult result = table.Execute(retrieveOperation);
            var tenantTbl = result.Result as TenantTbl;
            if (tenantTbl == null) return null;
           

            var prop = this.GetPropertyByCode(propCode);


            var pci = JsonConvert.DeserializeObject<PersonContactInfo>(tenantTbl.ContactInfo);

            IQueryable<TenantRequestTbl> linqQuery = _dbContext.RequestCloudTable.CreateQuery<TenantRequestTbl>()
                .Where(x => x.PartitionKey == pci.EmailAddress  )
                .Select(x => x);

            int count = linqQuery.ToList() .Count();

            return _entityMapper.CopyFrom(tenantTbl,count, _entityMapper.CopyFrom( prop) );
        }

        public IEnumerable<TenantRequest> GetTenantRequests(Guid tenantId)
        {
            IQueryable<TenantRequestTbl> linqQuery = _dbContext.RequestCloudTable.CreateQuery<TenantRequestTbl>()
                .Where(x => x.TenantID  == tenantId)
                .Select(x => x);
            var list = linqQuery.ToList();

            
            return list.Select(x => _entityMapper.CopyFrom(x, GetTenantByUnitNumber(x.UnitNumber,x.PropertyCode  ))).ToList();
        }

        public TenantRequest GetTenantRequest(string propCode, string tenantUnit, string requestCode)
        {
            var tenant = this.GetTenantByUnitNumber(tenantUnit,propCode);

            TableOperation retrieveOperation = TableOperation.Retrieve<TenantRequestTbl>(tenant.ContactInfo.EmailAddress , requestCode);
            var table = _dbContext.RequestCloudTable;

            TableResult result = table.Execute(retrieveOperation);
            var requestTbl = result.Result as TenantRequestTbl;
            if (requestTbl == null) return null;
            return _entityMapper.CopyFrom(requestTbl, tenant);
        }

        private TenantRequestTbl GetTenantRequestTbl(string tenantEmail, Guid requestId)
        {
            IQueryable<TenantRequestTbl> linqQuery = _dbContext.RequestCloudTable.CreateQuery<TenantRequestTbl>()
                .Where(x => x.PartitionKey == tenantEmail && x.ID == requestId )
                .Select(x => x);
            var list = linqQuery.ToList();
            return list.FirstOrDefault();
        }
        public IEnumerable<Worker> GetAllWorkers()
        {
            IQueryable<WorkerTbl> linqQuery = _dbContext.WorkerCloudTable .CreateQuery<WorkerTbl>() 
                .Where(x => x.PartitionKey == WorkerTbl.PartitionKeyConst )
                .Select(x => x);
            var list = linqQuery.ToList();
            return list.Select(x => _entityMapper.CopyFrom(x)).ToList();
        }

        public IEnumerable<TenantRequest> GetWorkerRequests(string workerEmail)
        {
            IQueryable<WorkerRequestTbl> linqQuery = _dbContext.WorkerRequestsCloudTable.CreateQuery<WorkerRequestTbl>()
                .Where(x => x.PartitionKey == workerEmail)
                .Select(x => x);
            var list = linqQuery.ToList();

            var list1 = list.Select(x => GetTenantRequestTbl(x.TenantEmail, x.RequestId)).ToList();

            return list1.Select(x => _entityMapper.CopyFrom(x, GetTenantByUnitNumber(x.UnitNumber, x.PropertyCode))).ToList();
        }

        public Worker GetWorkerByEmail(string email)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<WorkerTbl>(WorkerTbl.PartitionKeyConst, email);

            TableResult result = _dbContext.WorkerCloudTable.Execute(retrieveOperation);
            var workerTbl = result.Result as WorkerTbl;

            return workerTbl == null ? null : _entityMapper.CopyFrom(workerTbl);
        }
        
    }
}
