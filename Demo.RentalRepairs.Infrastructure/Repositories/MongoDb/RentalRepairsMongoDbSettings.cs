using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb
{
    public class RentalRepairsMongoDbSettings : IMongoDbSettings
    {
     
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
