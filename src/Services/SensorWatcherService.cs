using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;
using SensorWebApi.Services;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Service that watches for changes in the sensor measurements collection and sends updates to clients managed by <see cref="SensorHub"/>.
/// </summary>
public class SensorWatcherService
{
    private readonly ILogger<SensorWatcherService> _logger;
    private readonly MongoDbContext _context;
    private readonly IHubContext<SensorHub> _hubContext;
    private ConcurrentDictionary<string, byte> _watchedSensors;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _watchTask;

    public SensorWatcherService(
        ILogger<SensorWatcherService> logger,
        MongoDbContext context,
        IHubContext<SensorHub> hubContext)
    {
        _logger = logger;
        _context = context;
        _hubContext = hubContext;
        _watchedSensors = new ConcurrentDictionary<string, byte>();
        _cancellationTokenSource = new CancellationTokenSource();
        _watchTask = RunAsync(_cancellationTokenSource.Token); // Start the initial change stream
        _logger.LogInformation("SensorWatcherService initialized.");
    }

    /// <summary>
    /// Adds a sensorId to the watch list and restarts the change stream. If the sensorId is already being watched, this method does nothing.
    /// </summary>
    public void AddSensorId(string sensorId)
    {
        bool added = _watchedSensors.TryAdd(sensorId, 0);

        if (added)
        {
            _logger.LogInformation("Added sensorId {SensorId} to watch list.", sensorId);
            RestartChangeStream();
        }
    }

    /// <summary>
    /// Removes a sensorId from the watch list and restarts the change stream. If the sensorId is not being watched, this method does nothing.
    /// </summary>
    public void RemoveSensorId(string sensorId)
    {
        bool removed = _watchedSensors.TryRemove(sensorId, out _);
        if (removed)
        {
            _logger.LogInformation("Removed sensorId {SensorId} from watch list.", sensorId);
            RestartChangeStream();
        }
    }

    /// <summary>
    /// Restarts the change stream with the updated list of watched sensors.
    /// </summary>
    private async void RestartChangeStream()
    {
        _logger.LogInformation("Restarting change stream.");
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        await _watchTask;
        _watchTask = RunAsync(_cancellationTokenSource.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var col = _context.SensorMeasurements;

        while (!cancellationToken.IsCancellationRequested && _watchedSensors.Count > 0)
        {
            try
            {
                _logger.LogInformation("Starting change stream to watch [{WatchedSensors}].", string.Join(", ", _watchedSensors.Keys));

                var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<SensorMeasurements>>()
                    .Match(change =>
                        change.OperationType == ChangeStreamOperationType.Insert &&
                        _watchedSensors.ContainsKey(change.FullDocument.SensorId));

                var options = new ChangeStreamOptions
                {
                    FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
                };

                using var cursor = await col.WatchAsync(pipeline, options, cancellationToken);

                _logger.LogInformation("Watching for changes in sensor measurements collection.");

                await cursor.ForEachAsync(async change =>
                {

                    var sensorMeasurement = change.FullDocument;
                    var sensorId = sensorMeasurement.SensorId;

                    _logger.LogInformation("Change stream detected for sensorId {SensorId}.", sensorId);

                    if (_watchedSensors.TryGetValue(sensorId, out var clients))
                    {
                        _logger.LogDebug("Sending update for sensorId {SensorId} to clients.", sensorId);

                        await _hubContext.Clients.Group(sensorId)
                            .SendAsync("ReceiveSensorUpdate", sensorMeasurement);

                        _logger.LogInformation("Update sent for sensorId {SensorId}.", sensorId);
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Change stream canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in change stream: {ex.Message}");
                await Task.Delay(1000, cancellationToken); // Brief delay before retrying
            }
        }
    }
}
