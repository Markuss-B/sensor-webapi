using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using MongoDB.Driver;
using SensorWebApi.Models;
using SensorWebApi.Models.Configuration;

namespace SensorWebApi.Data;

public class MongoDb
{
    public MongoDb(IOptions<MongoDbSettings> options)
    {
        MongoDbSettings settings = options.Value;
        MongoClient client = new MongoClient(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

        Sensors = database.GetCollection<Sensor>("sensors");
        SensorMeasurements = database.GetCollection<SensorMeasurements>("sensorMeasurements");
        SensorMetadatas = database.GetCollection<SensorMetadatas>("sensorMetadatas");

        Notifications = database.GetCollection<Notification>("notifications");
        NotificationRules = database.GetCollection<NotificationRule>("notificationRules");

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
        };
        ConventionRegistry.Register("camelCase", conventionPack, t => true);
    }

    public IMongoCollection<Sensor> Sensors { get; set; }
    public IMongoCollection<SensorMeasurements> SensorMeasurements { get; set; }
    public IMongoCollection<SensorMetadatas> SensorMetadatas { get; set; }

    public IMongoCollection<Notification> Notifications { get; set; }
    public IMongoCollection<NotificationRule> NotificationRules { get; set; }
}