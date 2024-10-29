using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;
using SensorWebApi.Services;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class SensorWatcherService
{
    private readonly ILogger<SensorWatcherService> _logger;
    private readonly MongoDbContext _context;
    private readonly IHubContext<SensorHub> _hubContext;
    private readonly ConcurrentDictionary<string, HashSet<string>> _sensorClientMap; // Tracks clients for each sensor
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
        _sensorClientMap = new ConcurrentDictionary<string, HashSet<string>>();
        _cancellationTokenSource = new CancellationTokenSource();
        _watchTask = RunAsync(_cancellationTokenSource.Token); // Start the initial change stream
        _logger.LogInformation("SensorWatcherService initialized.");
    }

    public void AddSensorId(string sensorId, string clientId)
    {
        // Add or update the sensor-client mapping
        _sensorClientMap.AddOrUpdate(sensorId,
            new HashSet<string> { clientId },
            (key, clients) =>
            {
                clients.Add(clientId);
                return clients;
            });

        _logger.LogInformation("Added sensorId {SensorId} for clientId {ClientId}.", sensorId, clientId);
        RestartChangeStream();
    }

    public void RemoveSensorId(string sensorId, string clientId)
    {
        if (_sensorClientMap.TryGetValue(sensorId, out var clients))
        {
            clients.Remove(clientId);

            if (clients.Count == 0)
            {
                _sensorClientMap.TryRemove(sensorId, out _);
                _logger.LogInformation("Removed sensorId {SensorId} as there are no more clients.", sensorId);
                RestartChangeStream();
            }
            else
            {
                _logger.LogInformation("Removed clientId {ClientId} from sensorId {SensorId}.", clientId, sensorId);
            }
        }
    }

    private void RestartChangeStream()
    {
        _logger.LogInformation("Restarting change stream.");
        _cancellationTokenSource.Cancel(); // Cancel the current change stream
        _cancellationTokenSource = new CancellationTokenSource(); // Create a new cancellation token
        _watchTask = RunAsync(_cancellationTokenSource.Token); // Restart the change stream with the updated list
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var col = _context.SensorMeasurements;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<SensorMeasurements>>()
                    .Match(change =>
                        change.OperationType == ChangeStreamOperationType.Insert &&
                        _sensorClientMap.ContainsKey(change.FullDocument.SensorId));

                var options = new ChangeStreamOptions
                {
                    FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
                };

                using var cursor = await col.WatchAsync(pipeline, options, cancellationToken);

                await cursor.ForEachAsync(async change =>
                {
                    var sensorMeasurement = change.FullDocument;
                    var sensorId = sensorMeasurement.SensorId;

                    if (_sensorClientMap.TryGetValue(sensorId, out var clients))
                    {
                        _logger.LogInformation("Sending update for sensorId {SensorId} to clients.", sensorId);
                        await _hubContext.Clients.Group(sensorId)
                            .SendAsync("ReceiveSensorUpdate", sensorMeasurement);
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Change stream restarted due to sensor list update.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in change stream: {ex.Message}");
                await Task.Delay(1000, cancellationToken); // Brief delay before retrying
            }
        }
    }
}
