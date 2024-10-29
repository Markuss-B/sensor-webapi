using Microsoft.AspNetCore.SignalR;

public class SensorHub : Hub
{
    private readonly SensorWatcherService _sensorWatcherService;

    public SensorHub(SensorWatcherService sensorWatcherService)
    {
        _sensorWatcherService = sensorWatcherService;
    }
    public async Task SubscribeToSensor(string sensorId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sensorId);

        _sensorWatcherService.AddSensorId(sensorId, Context.ConnectionId);
    }

    public async Task UnsubscribeFromSensor(string sensorId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sensorId);

        _sensorWatcherService.RemoveSensorId(sensorId, Context.ConnectionId);
    }
}
