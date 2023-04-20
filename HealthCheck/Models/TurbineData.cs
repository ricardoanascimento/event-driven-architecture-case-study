using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TurbineData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    public string TurbineId { get; set; }
    public float Volt { get; set; }
    public float Amp { get; set; }
    public int RPM { get; set; }
    public DateTime TimeStamp { get; set; }
}