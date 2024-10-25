namespace SensorWebApi.Models;

public class Sensor
{
    public required string Id { get; set; } // Unique identifier for each sensor
    public string? BaseSerialNumber { get; set; }
    public string? RootTopic { get; set; }
    public string? Name { get; set; }
    public string? ProductNumber { get; set; }
    public string? Group { get; set; }
    public string? GroupId { get; set; }
    public List<SensorMeasurements>? SensorMeasurements { get; set; }
}
