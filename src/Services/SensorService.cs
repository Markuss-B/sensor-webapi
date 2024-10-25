using Microsoft.EntityFrameworkCore;
using SensorWebApi.Data;
using SensorWebApi.Models;

namespace SensorWebApi.Services;

public class SensorService
{
    private readonly SensorDbContext _context;

    public SensorService(SensorDbContext context)
    {
        _context = context;
    }

    public IQueryable<Sensor> SensorQuery() => _context.Sensors.AsNoTracking();

    public List<Sensor> GetSensors()
    {
        return _context.Sensors
            .AsNoTracking()
            .ToList();
    }

    public Sensor? GetSensorById(string id)
    {
        return _context.Sensors
            .AsNoTracking()
            .FirstOrDefault(s => s.Id == id);
    }

    public SensorMeasurements? GetLatestSensorMeasurment(string sensorId)
    {
        return _context.SensorMeasurements
            .AsNoTracking()
            .Where(s => s.SensorId == sensorId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefault();
    }

    public IOrderedQueryable<SensorMeasurements> GetSensorMeasurments(string sensorId, DateTime? dateFrom, DateTime? dateTo)
    {
        var query = _context.SensorMeasurements
            .AsNoTracking()
            .Where(s => s.SensorId == sensorId);

        if (dateFrom.HasValue)
        {
            query = query.Where(s => s.Timestamp >= dateFrom);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(s => s.Timestamp <= dateTo);
        }

        return query
            .OrderByDescending(s => s.Timestamp);
    }
}
