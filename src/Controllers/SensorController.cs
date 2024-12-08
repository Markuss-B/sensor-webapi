using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorWebApi.Models;
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

    
    [HttpGet("{sensorId}/measurements/today")]
    public ActionResult<List<SensorMeasurements>> GetTodaysSensorMeasurements(string sensorId)
    {
        var sensor = _sensorService.GetSensorById(sensorId);
        if (sensor == null)
            return NotFound();

        var measurements = _sensorService.GetSensorMeasurements(sensorId, DateTime.Today);
        return Ok(measurements);
    }

    // PUT: api/sensor
    [HttpPut]
    public ActionResult Put(SensorUpdateDto sensor)
    {
        // check if sensor exists
        var existingSensor = _sensorService.GetSensorById(sensor.Id);
        if (existingSensor == null)
            return NotFound("Sensor not found");

        if (NoChangesToSensor(existingSensor, sensor))
            return Ok("No changes to sensor");

        var success = _sensorService.UpdateSensor(sensor);

        if (!success)
            return BadRequest();

        return Ok("Sensor updated");
    }

    private bool NoChangesToSensor(Sensor sensor, SensorUpdateDto sensorUpdate)
    {
        return sensor.IsActive == sensorUpdate.IsActive &&
               sensor.Location == sensorUpdate.Location;
    }

    //// GET: api/Sensor/5/latestmeasurment
    //[HttpGet("{sensorId}/measurements/latest")]
    //public ActionResult<SensorMeasurements> GetLatestMeasurment(string sensorId)
    //{
    //    var latestMeasurment = _sensorService.GetLatestSensorMeasurment(sensorId);
    //    if (latestMeasurment == null)
    //        return NotFound();
    //    return Ok(latestMeasurment);
    //}

    //// GET: api/Sensor/5/measurements
    //[HttpGet("{sensorId}/measurements")]
    //public ActionResult<List<SensorMeasurements>> GetMeasurements(string sensorId, DateTime? dateFrom, DateTime? dateTo)
    //{
    //    var sensor = _sensorService.GetSensorById(sensorId);
    //    if (sensor == null)
    //        return NotFound();

    //    var measurements = _sensorService.GetSensorMeasurements(sensorId, dateFrom, dateTo);
    //    return Ok(measurements);
    //}

    //// GET: api/measurmentCounts
    //[HttpGet("measurmentCounts")]
    //public ActionResult<List<MeasurementCountDto>> GetMeasurmentCounts()
    //{
    //    var measurmentCounts = _sensorService.GetSensorMeasurementsCount();

    //    return Ok(measurmentCounts);
    //}
}
