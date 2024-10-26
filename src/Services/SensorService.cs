using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;
using SensorWebApi.Models.Dto;

namespace SensorWebApi.Services;

public class SensorService
{
    private readonly MongoDbContext _context;

    public SensorService(MongoDbContext context)
    {
        _context = context;
    }

    public List<Sensor> GetSensors()
    {
        return _context.Sensors
            .Find(FilterDefinition<Sensor>.Empty)
            .ToList();
    }

    public Sensor? GetSensorById(string id)
    {
        return _context.Sensors
            .Find(s => s.Id == id)
            .FirstOrDefault();
    }

    public SensorMeasurements? GetLatestSensorMeasurment(string sensorId)
    {
        return _context.SensorMeasurements
            .Find(s => s.SensorId == sensorId)
            .SortByDescending(s => s.Timestamp)
            .FirstOrDefault();
    }

    public List<SensorMeasurements> GetSensorMeasurments(string sensorId, DateTime? dateFrom, DateTime? dateTo)
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

        return _context.SensorMeasurements
            .Find(filter)
            .SortByDescending(s => s.Timestamp)
            .ToList();
    }

    public List<MeasurementCountDto> GetSensorMeasurementsCount()
    {
        return null;
    }
}
