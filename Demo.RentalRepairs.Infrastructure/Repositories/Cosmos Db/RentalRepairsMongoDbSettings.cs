using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db
{
    public class RentalRepairsCosmosDbSettings : IMongoDbSettings
    {

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
