using MongoDB.Driver;
using SharedKernel.Services;
using SharedKernel.Models;

public class EventAggregatorDataHandler : MongoDBDataHandler<TurbineDataAggregateEntity>, IDataHandler<TurbineDataAggregateEntity>
{
    public EventAggregatorDataHandler(string connectionString, string databaseName, string collectionName)
    : base(connectionString, databaseName, collectionName)
    { }

    public async Task InsertAsync(TurbineDataAggregateEntity document)
    {
        await _collection.InsertOneAsync(document);
    }

    public async Task<TurbineDataAggregateEntity> GetLatestByTurbineId(string turbineId, DateTime earliestDate)
    {
        var filter = Builders<TurbineDataAggregateEntity>.Filter.And(
                   Builders<TurbineDataAggregateEntity>.Filter.Eq(doc => doc.TurbineId, turbineId),
                   Builders<TurbineDataAggregateEntity>.Filter.Gte(doc => doc.CreatedAt, earliestDate)
                   );

        var options = new FindOptions<TurbineDataAggregateEntity, TurbineDataAggregateEntity>()
        {
            Sort = Builders<TurbineDataAggregateEntity>.Sort.Descending("UpdatedAt")
        };

        var cursor = await _collection.FindAsync(filter, options);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task ReplaceOneAsync(TurbineDataAggregateEntity latestAggregateDocument)
    {
        var filter = Builders<TurbineDataAggregateEntity>.Filter.Eq("_id", latestAggregateDocument._id);
        await _collection.ReplaceOneAsync(filter, latestAggregateDocument);
    }
}