using SharedKernel.Models;
using MongoDB.Driver;

namespace SharedKernel.Services
{
    public class MongoDBDataHandler<T>
    {
        protected readonly IMongoClient _client;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _collection;

        public MongoDBDataHandler(string connectionString, string databaseName, string collectionName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            _collection = _database.GetCollection<T>(collectionName);
        }
    }
}