namespace SensorWebApi.Models.Dto;

public class MeasurementCountDto
{
    public required string SensorId { get; set; }
    public string? SensorName { get; set; }
    public int MeasurmentCount { get; set; }
}
