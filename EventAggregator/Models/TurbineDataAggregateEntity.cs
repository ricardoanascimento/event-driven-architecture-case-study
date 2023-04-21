using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedKernel.Models;

public class TurbineDataAggregateEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    public string? TurbineId { get; set; }
    public float TotalVolts { get; set; }
    public List<TurbineData>? Telemetries { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}