using MongoDB.Driver;

namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; set; }
        void DropDb();

    }
}