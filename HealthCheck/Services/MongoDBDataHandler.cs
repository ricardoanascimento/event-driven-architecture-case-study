using MongoDB.Driver;

public class MongoDBDataHandler : IDataHandler
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TurbineData> _collection;

    public MongoDBDataHandler(string connectionString, string databaseName, string collectionName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
        _collection = _database.GetCollection<TurbineData>(collectionName);
    }

    public async Task<List<TurbineData>> GetListWithinTimeFrameByTurbineIdAsync(string turbineId, DateTime earliestDate)
    {
        var filter = Builders<TurbineData>.Filter.And(
            Builders<TurbineData>.Filter.Eq(doc => doc.TurbineId, turbineId),
            Builders<TurbineData>.Filter.Gte(doc => doc.TimeStamp, earliestDate));

        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task InsertAsync(TurbineData document)
    {
        await _collection.InsertOneAsync(document);
    }
}