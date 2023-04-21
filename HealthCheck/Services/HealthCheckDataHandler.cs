using MongoDB.Driver;
using SharedKernel.Models;
using SharedKernel.Services;

public class HealthCheckDataHandler : MongoDBDataHandler<TurbineDataEntity>, IDataHandler<TurbineDataEntity>
{
    public HealthCheckDataHandler(string connectionString, string databaseName, string collectionName)
    : base(connectionString, databaseName, collectionName)
    { }

    public async Task InsertAsync(TurbineDataEntity document)
    {
        await _collection.InsertOneAsync(document);
    }

    public async Task<List<TurbineDataEntity>> GetListWithinTimeFrameByTurbineIdAsync(string turbineId, DateTime earliestDate)
    {
        var filter = Builders<TurbineDataEntity>.Filter.And(
            Builders<TurbineDataEntity>.Filter.Eq(doc => doc.TurbineId, turbineId),
            Builders<TurbineDataEntity>.Filter.Gte(doc => doc.TimeStamp, earliestDate)
            );

        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }
}
