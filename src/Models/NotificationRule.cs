using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class NotificationRule
{
    [BsonId]
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? SensorId { get; set; }
    public string Measurement { get; set; }
    public string Operator { get; set; }
    public double Value { get; set; }
    [BsonIgnore]
    public List<Notification> Notifications { get; set; }
}
