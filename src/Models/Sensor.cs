using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

[BsonIgnoreExtraElements]
public class Sensor
{
    /// <summary>
    /// Unique ID for the sensor.
    /// </summary>
    [BsonId]
    public string Id { get; set; }

    /// <summary>
    /// Topics that the sensor sends data to.
    /// </summary>
    [BsonElement("topics")]
    public List<string>? Topics { get; set; }
    /// <summary>
    /// Is the sensor active.
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Location of the sensor set by the user.
    /// </summary>
    [BsonElement("location")]
    public string? Location { get; set; }
    /// <summary>
    /// Metadata of the sensor which is received from the sensor by MQTT.
    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; }
    /// <summary>
    /// Lastest measurement timestamp.
    /// </summary>
    [BsonElement("latestMeasurementTimestamp")]
    public DateTime LatestMeasurementTimestamp { get; set; } // Time the measurement was recorded
    /// <summary>
    /// Latest measurements from the sensor.
    /// </summary>
    [BsonElement("latestMeasurements")]
    public Dictionary<string, object> LatestMeasurements { get; set; } // Dictionary for varied measurement fields
    /// <summary>
    /// Description field which is set by the user.
    /// 
    [BsonElement("description")]
    public string? Description { get; set; }
}

public class SensorUpdateDto
{
    public string Id { get; set; }
    public string? Location { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
}
