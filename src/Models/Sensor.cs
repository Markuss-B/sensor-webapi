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
    /// Sensor name set by the sensor.
    /// </summary>
    [BsonElement("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Product number of the sensor set by the sensor.
    /// </summary>
    [BsonElement("productNumber")]
    public string? ProductNumber { get; set; }

    /// <summary>
    /// Group identifier for the sensor set by the sensor.
    /// </summary>
    [BsonElement("group")]
    public string? Group { get; set; }

    /// <summary>
    /// Group ID of the sensor set by the sensor.
    /// </summary>
    [BsonElement("groupId")]
    public string? GroupId { get; set; }
    /// <summary>
    /// Is the sensor active.
    /// </summary>
    [BsonElement("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Location of the sensor set by the user.
    /// </summary>
    [BsonElement("location")]
    public string? Location { get; set; }

    /// <summary>
    /// Last update time of the sensor.
    /// </summary>
    [BsonElement("lastUpdate")]
    public DateTime? LastUpdate { get; set; }
}

public class SensorUpdateDto
{
    public string Id { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }
}
