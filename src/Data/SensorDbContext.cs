using Microsoft.EntityFrameworkCore;
using SensorWebApi.Models;

namespace SensorWebApi.Data;

public class SensorDbContext : DbContext
{
    public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options)
    {
    }

    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorMeasurements> SensorMeasurements { get; set; }
}
