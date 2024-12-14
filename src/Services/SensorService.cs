using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;

namespace SensorWebApi.Services;

public class SensorService
{
    private readonly MongoDb _db;

    public SensorService(MongoDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all sensors.
    /// </summary>
    public List<Sensor> GetSensors()
    {
        return _db.Sensors
            .Find(FilterDefinition<Sensor>.Empty)
            .ToList();
    }

    /// <summary>
    /// Gets sensor by it's id.
    /// </summary>
    public Sensor? GetSensorById(string sensorId)
    {
        return _db.Sensors
            .Find(s => s.Id == sensorId)
            .FirstOrDefault();
    }

    //public SensorMeasurements? GetLatestSensorMeasurment(string sensorId)
    //{
    //    return _context.SensorMeasurements
    //        .Find(s => s.SensorId == sensorId)
    //        .SortByDescending(s => s.Timestamp)
    //        .FirstOrDefault();
    //}

    /// <summary>
    /// Get sensor measurements for a specific sensor during a specific time period.
    /// If dateFrom is not provided, then only 100 records are returned.
    /// </summary>
    public List<SensorMeasurements> GetSensorMeasurements(string sensorId, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var filterBuilder = Builders<SensorMeasurements>.Filter;
        var filter = filterBuilder.Eq(s => s.SensorId, sensorId);

        if (dateFrom.HasValue)
        {
            filter &= filterBuilder.Gte(s => s.Timestamp, dateFrom);
        }

        if (dateTo.HasValue)
        {
            filter &= filterBuilder.Lte(s => s.Timestamp, dateTo);
        }

        var query = _db.SensorMeasurements
            .Find(filter);

        if (!dateFrom.HasValue && !dateTo.HasValue)
        {
            query = query.Limit(100);
        }

        return query
            .SortBy(s => s.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Updates a sensor. Currently only allows updates on the location and isActive.
    /// </summary>
    /// <param name="sensor"></param>
    /// <returns></returns>
    public bool UpdateSensor(SensorUpdateDto sensor, Sensor oldSensor)
    {
        var filter = Builders<Sensor>.Filter.Eq(s => s.Id, sensor.Id);

        var update = Builders<Sensor>.Update
            .Set(s => s.Location, sensor.Location)
            .Set(s => s.IsActive, sensor.IsActive);

        var result = _db.Sensors.UpdateOne(filter, update);

        if (result.ModifiedCount > 0)
        {
            if (sensor.Location != oldSensor.Location)
            {
                SaveMetadataHistory(sensor.Id, nameof(sensor.Location), sensor.Location);
            }

            if (sensor.IsActive != oldSensor.IsActive)
            {
                SaveMetadataHistory(sensor.Id, nameof(sensor.IsActive), sensor.IsActive.ToString());
            }
        }

        return true;
    }

    private void SaveMetadataHistory(string sensorId, string field, string newValue)
    {
        var history = new SensorMetadatas
        {
            SensorId = sensorId,
            Timestamp = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { field, newValue }
            }
        };
        _db.SensorMetadatas.InsertOne(history);
    }
}
