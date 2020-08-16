using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;
using MongoDB.Driver;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb
{
    public class RentalRepairsMongoDbContext : IMongoDbContext
    {
        private readonly IMongoDbSettings _connectionSettings;
        private readonly MongoClient _mongoClient;

        public IMongoDatabase Database { get; set; }
        public void DropDb()
        {
             _mongoClient.DropDatabase(_connectionSettings.DatabaseName);
         
        }


        public RentalRepairsMongoDbContext(IMongoDbSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            _mongoClient = new MongoClient(connectionSettings.ConnectionString);//"mongodb://localhost:27017");
            //string connectionString =
            //    @"mongodb://demo-rental-repairs:v2dYhXLCVoI504XNoWgOE9B7Ry0ayfX2Z6uk2sVezysix2fXtKZHM2wcCm8f8lDKUapgQrXfip26vNqXwWPBOA==@demo-rental-repairs.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@demo-rental-repairs@";

            //MongoClientSettings settings = MongoClientSettings.FromUrl(
            //    new MongoUrl(connectionString)
            //);
            //settings.SslSettings =
            //    new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            //var mongoClient = new MongoClient(settings);

            //var database = client.GetDatabase("RentalRepairs");
            Database = _mongoClient.GetDatabase(connectionSettings.DatabaseName );
            


        }

    }
}
