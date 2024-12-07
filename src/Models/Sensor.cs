﻿using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class Sensor
{
    [BsonId]
    public string Id { get; set; } // Unique ID for the sensor
    [BsonElement("baseSerial")]
    public string? BaseSerialNumber { get; set; } // Serial number of the base unit
    [BsonElement("rootTopic")]
    public string? RootTopic { get; set; } // Root topic for the sensor
    [BsonElement("name")]
    public string? Name { get; set; } // Sensor name
    [BsonElement("productNumber")]
    public string? ProductNumber { get; set; } // Product number of the sensor
    [BsonElement("group")]
    public string? Group { get; set; } // Group identifier for the sensor
    [BsonElement("groupId")]
    public string? GroupId { get; set; } // Group ID of the sensor
    [BsonElement("location")]
    public string? Location { get; set; } // Location of the sensor. Usually set by user.
}

