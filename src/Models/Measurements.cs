using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

[BsonIgnoreExtraElements]
public class Measurements
{
    // measurements object co2, temperature ...
    [BsonElement("co2")]
    public int? Co2 { get; set; }
    [BsonElement("atmosphericpressure")]
    public int? AtmosphericPressure { get; set; }
    [BsonElement("battery")]
    public double? Battery { get; set; }
    [BsonElement("temperature")]
    public double? Temperature { get; set; }
    [BsonElement("humidity")]
    public double? Humidity { get; set; }
    [BsonElement("rssi")]
    public double? Rssi { get; set; }
}
