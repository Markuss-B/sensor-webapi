using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorWebApi.Models;
using SensorWebApi.Models.Dto;
using SensorWebApi.Services;

namespace SensorWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SensorController : ControllerBase
{
    private readonly SensorService _sensorService;

    public SensorController(SensorService sensorService)
    {
        _sensorService = sensorService;
    }

    // GET: api/Sensor
    [HttpGet]
    public ActionResult<List<Sensor>> Get()
    {
        var sensors = _sensorService.GetSensors();
        return Ok(sensors);
    }

    // GET: api/Sensor/5
    [HttpGet("{sensorId}")]
    public ActionResult<Sensor> Get(string sensorId)
    {
        var sensor = _sensorService.GetSensorById(sensorId);
        if (sensor == null)
            return NotFound();
        return Ok(sensor);
    }

    // GET: api/Sensor/5/latestmeasurment
    [HttpGet("{sensorId}/latestmeasurment")]
    public ActionResult<SensorMeasurements> GetLatestMeasurment(string sensorId)
    {
        var latestMeasurment = _sensorService.GetLatestSensorMeasurment(sensorId);
        if (latestMeasurment == null)
            return NotFound();
        return Ok(latestMeasurment);
    }

    // GET: api/Sensor/5/measurments
    [HttpGet("{sensorId}/measurments")]
    public ActionResult<List<SensorMeasurements>> GetMeasurments(string sensorId, DateTime? dateFrom, DateTime? dateTo)
    {
        var sensor = _sensorService.GetSensorById(sensorId);
        if (sensor == null)
            return NotFound();

        var measurments = _sensorService.GetSensorMeasurments(sensorId, dateFrom, dateTo);
        return Ok(measurments);
    }

    // GET: api/measurmentCounts
    [HttpGet("measurmentCounts")]
    public ActionResult<List<MeasurmentCountDto>> GetMeasurmentCounts()
    {
        var measurmentCounts = _sensorService.SensorQuery()
            .Select(s => new
            {
                SensorId = s.Id,
                SensorName = s.Name,
                MeasurmentCount = s.SensorMeasurements != null ? s.SensorMeasurements.Count : 0
            })
            .ToList();

        return Ok(measurmentCounts);
    }
}
