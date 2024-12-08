using System.Collections.Concurrent;

namespace SensorWebApi.Services;

public class GroupTrackingService
{
    private readonly ILogger<GroupTrackingService> _logger;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _groupClientMap;

    public GroupTrackingService(ILogger<GroupTrackingService> logger)
    {
        _logger = logger;
        _groupClientMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();
    }

    /// <summary>
    /// Adds a client to a group and returns the new count of clients in the group.
    /// </summary>
    /// <returns>The new count of clients in the group.</returns>
    public int AddToGroup(string groupName, string clientId)
    {
        var value = _groupClientMap.AddOrUpdate(groupName,
            new ConcurrentDictionary<string, byte> { [clientId] = 0 },
            (_, clients) =>
            {
                clients.TryAdd(clientId, 0);
                return clients;
            });

        _logger.LogInformation("Added client {ClientId} to group {GroupName}. Group count: {Count}", clientId, groupName, value.Count);

        return value.Count;
    }

    /// <summary>
    /// Removes a client from a group and returns the new count of clients in the group.
    /// </summary>
    /// <returns>The new count of clients in the group.</returns>
    public int RemoveFromGroup(string groupName, string clientId)
    {
        if (_groupClientMap.TryGetValue(groupName, out var clients))
        {
            if (clients.Count <= 1)
            {
                _groupClientMap.TryRemove(groupName, out _);

                _logger.LogInformation("Removed group {GroupName}.", groupName);

                return 0;
            }
            else
            {
                clients.TryRemove(clientId, out _);

                _logger.LogInformation("Removed client from group {GroupName}. Group count: {Count}", groupName, clients.Count);

                return clients.Count;
            }
        }

        return 0;
    }

    /// <summary>
    /// Removes a client from all their groups.
    /// </summary>
    /// <param name="clientId"></param>
    public void RemoveClient(string clientId)
    {
        foreach (var group in _groupClientMap)
        {
            if (group.Value.TryRemove(clientId, out _))
            {
                _logger.LogInformation("Removed client {ClientId} from group {GroupName}. Group count: {Count}", clientId, group.Key, group.Value.Count);
            }
        }
    }
}
