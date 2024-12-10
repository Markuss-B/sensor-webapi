using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;

namespace SensorWebApi.Services;

public class SensorService
{
    private readonly MongoDbContext _context;

    public SensorService(MongoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all sensors.
    /// </summary>
    public List<Sensor> GetSensors()
    {
        return _context.Sensors
            .Find(FilterDefinition<Sensor>.Empty)
            .ToList();
    }

    /// <summary>
    /// Gets sensor by it's id.
    /// </summary>
    public Sensor? GetSensorById(string sensorId)
    {
        return _context.Sensors
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

        var query = _context.SensorMeasurements
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
    public bool UpdateSensor(SensorUpdateDto sensor)
    {
        var filter = Builders<Sensor>.Filter.Eq(s => s.Id, sensor.Id);

        var update = Builders<Sensor>.Update
            .Set(s => s.Location, sensor.Location)
            .Set(s => s.IsActive, sensor.IsActive);

        var result = _context.Sensors.UpdateOne(filter, update);

        return result.ModifiedCount > 0;
    }
}
