﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorWebApi.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    [BsonElement("message")]
    public string Message { get; set; }
    [BsonElement("startTimestamp")]
    public DateTime StartTimestamp { get; set; }
    [BsonElement("endTimestamp")]
    [BsonDefaultValue(null)]
    public DateTime? EndTimestamp { get; set; }
    [BsonElement("ruleId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RuleId { get; set; }
}
