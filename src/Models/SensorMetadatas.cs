﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class SensorMetadatas
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Unique identifier for each measurement record

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } // Time the measurement was recorded

    [BsonElement("sensorId")]
    public string SensorId { get; set; } // Sensor Id given by the sensor

    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; } // Dictionary for varied measurement fields
}