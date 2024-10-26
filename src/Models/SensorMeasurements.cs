using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using static SensorWebApi.Helpers.ObjectExtensions;

namespace SensorWebApi.Models;

public class SensorMeasurements
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Unique identifier for each measurement record
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } // Time the measurement was recorded
    [BsonElement("sensorId")]
    public string SensorId { get; set; } // Sensor Id given by the sensor
    [BsonElement("measurements")]
    [JsonConverter(typeof(BsonDocJsonConverter))]
    public BsonDocument Measurements { get; set; } // Measurements from the sensor
    /* Possible measurements from the sensor
    {
        "co2": "452",
        "atmosphericpressure": "102560",
        "time": "1729716854",
        "battery": "0.92",
        "temperature": "22.800",
        "humidity": "41.0",
        "rssi": "-69.000000"
    }
    */
}

public class SensorMeasurementsRaw
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("sensorId")]
    public string SensorId { get; set; }
    [BsonExtraElements]
    [JsonConverter(typeof(BsonDocJsonConverter))]
    public BsonDocument Measurements { get; set; }
}

