using Microsoft.AspNetCore.SignalR;
using SensorWebApi.Services;
using System.Collections.Concurrent;
using static MongoDB.Driver.WriteConcern;

public class SensorHub : Hub
{
    private readonly ILogger<SensorHub> _logger;
    private readonly SensorWatcherService _sensorWatcherService;
    private readonly GroupTrackingService _groupTrackingService;

    public SensorHub(ILogger<SensorHub> logger, SensorWatcherService sensorWatcherService, GroupTrackingService groupTrackingService)
    {
        _logger = logger;
        _sensorWatcherService = sensorWatcherService;
        _groupTrackingService = groupTrackingService;
    }
    public async Task SubscribeToSensor(string sensorId)
    {
        _logger.LogInformation("Received subscribe request from client {ConnectionId} for sensor {SensorId}.", Context.ConnectionId, sensorId);

        await Groups.AddToGroupAsync(Context.ConnectionId, sensorId);
        int count = _groupTrackingService.AddToGroup(sensorId, Context.ConnectionId);

        _logger.LogInformation("Client {ConnectionId} subscribed to sensor {SensorId}. Sensor subscriber count: {Count}.", Context.ConnectionId, sensorId, count);

        _sensorWatcherService.AddSensorId(sensorId);
    }

    public async Task UnsubscribeFromSensor(string sensorId)
    {
        _logger.LogInformation("Received unsubscribe request from client {ConnectionId} for sensor {SensorId}.", Context.ConnectionId, sensorId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sensorId);
        int count = _groupTrackingService.RemoveFromGroup(sensorId, Context.ConnectionId);

        _logger.LogInformation("Client {ConnectionId} unsubscribed from sensor {SensorId}. Sensor subscriber count: {Count}.", Context.ConnectionId, sensorId, count);

        if (count == 0)
            _sensorWatcherService.RemoveSensorId(sensorId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client {ConnectionId} disconnected.", Context.ConnectionId);

        _groupTrackingService.RemoveClient(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}
