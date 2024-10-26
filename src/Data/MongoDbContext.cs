using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SensorWebApi.Models;
using SensorWebApi.Models.Configuration;

namespace SensorWebApi.Data;

public class MongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        MongoDbSettings settings = options.Value;
        MongoClient client = new MongoClient(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

        Sensors = database.GetCollection<Sensor>("sensors");
        SensorMeasurements = database.GetCollection<SensorMeasurements>("sensorMeasurements");
        SensorMeasurementsRaw = database.GetCollection<SensorMeasurementsRaw>("sensorMeasurementsRaw");
    }

    public IMongoCollection<Sensor> Sensors { get; set; }
    public IMongoCollection<SensorMeasurements> SensorMeasurements { get; set; }
    public IMongoCollection<SensorMeasurementsRaw> SensorMeasurementsRaw { get; set; }
}