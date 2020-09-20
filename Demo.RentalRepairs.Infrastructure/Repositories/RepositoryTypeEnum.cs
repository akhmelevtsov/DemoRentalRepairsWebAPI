using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Infrastructure.Repositories
{
    public enum RepositoryTypeEnum
    {
        InMemory,
        SqlServer,
        MongoDb,
        CosmosDb,
        AzureTable
    }
}
