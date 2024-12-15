using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class Notification
{
    [BsonId]
    public string Id { get; set; }
    public string? SensorId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    [BsonDefaultValue(false)]
    public string RuleId { get; set; }
}
