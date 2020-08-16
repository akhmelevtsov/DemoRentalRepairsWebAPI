using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Models;
using MongoDB.Driver;

namespace Demo.RentalRepairs.Infrastructure.Repositories.Cosmos_Db
{
    public class RentalRepairsCosmosDbContext : IMongoDbContext
    {
        private readonly IMongoDbSettings _connectionSettings;
        private readonly MongoClient _mongoClient;
        public IMongoDatabase Database { get; set; }
        public void DropDb()
        {
            _mongoClient.DropDatabase(_connectionSettings.DatabaseName);

        }

        public RentalRepairsCosmosDbContext(IMongoDbSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionSettings.ConnectionString)
            );
            settings.SslSettings =
                new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            _mongoClient = new MongoClient(settings);

           
            Database = _mongoClient.GetDatabase(connectionSettings.DatabaseName);

        }

    }
}
