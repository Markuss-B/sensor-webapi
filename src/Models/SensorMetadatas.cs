using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class SensorMetadatas
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    /// <summary>
    /// Time the metadata change was recorded
    /// </summary>
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Sensor Id given by the sensor
    /// </summary>
    [BsonElement("sensorId")]
    public string SensorId { get; set; }

    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; }
}
