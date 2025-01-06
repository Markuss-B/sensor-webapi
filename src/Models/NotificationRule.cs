using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class NotificationRule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    /// <summary>
    /// Name of the rule which will appear in the message
    /// </summary>
    [BsonElement("name")]
    public string? Name { get; set; }
    /// <summary>
    /// Limits the rule to a specific sensor.
    /// </summary>
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    /// <summary>
    /// Measurement to compare ex. temperature, co2
    /// </summary>
    [BsonElement("measurement")]
    public string Measurement { get; set; }
    /// <summary>
    /// Operator to compare the value with the measurement. ">", "<"
    /// </summary>
    [BsonElement("operator")]
    public string Operator { get; set; }
    /// <summary>
    /// Value to compare with the measurement. If the operator is ">" and the value is 30, the rule will trigger if the measurement is over 30.
    /// </summary>
    [BsonElement("value")]
    public double Value { get; set; }
    /// <summary>
    /// Used when returning a singular rule.
    /// </summary>
    [BsonIgnore]
    [BsonElement("notifications")]
    public List<Notification> Notifications { get; set; }
}
