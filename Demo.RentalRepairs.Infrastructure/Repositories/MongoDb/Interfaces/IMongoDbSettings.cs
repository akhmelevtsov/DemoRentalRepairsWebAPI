namespace Demo.RentalRepairs.Infrastructure.Repositories.MongoDb.Interfaces
{
    public interface IMongoDbSettings
    {
      
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}