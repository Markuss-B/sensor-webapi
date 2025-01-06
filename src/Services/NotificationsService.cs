using MongoDB.Driver;
using SensorWebApi.Data;
using SensorWebApi.Models;

namespace SensorWebApi.Services;

public class NotificationsService
{
    private readonly MongoDb _db;

    public NotificationsService(MongoDb db)
    {
        _db = db;
    }

    public IQueryable<NotificationRule> NotificationRules => _db.NotificationRules.AsQueryable();

    // Notifications
    /// <summary>
    /// Get all notifications in descending order.
    /// </summary>
    public List<Notification> GetNotifications()
    {
        return _db.Notifications
            .Find(FilterDefinition<Notification>.Empty)
            .SortByDescending(n => n.StartTimestamp)
            .ToList();
    }

    // Notification Rules

    /// <summary>
    /// Get all notification rules.
    /// </summary>
    /// <returns></returns>
    public List<NotificationRule> GetNotificationRules()
    {
        return _db.NotificationRules
            .Find(FilterDefinition<NotificationRule>.Empty)
            .ToList();
    }

    /// <summary>
    /// Get a specific notification rule by its id.
    /// </summary>
    /// <param name="ruleId">Rule ID as it in the database</param>
    /// <returns><see cref="NotificationRule"/> or null if the rule doesn't exist</returns>
    public NotificationRule? GetNotificationRule(string ruleId)
    {
        var rule = _db.NotificationRules.AsQueryable()
            .FirstOrDefault(r => r.Id == ruleId);

        // Rule doesn't exist
        if (rule == null)
            return null;

        // Get notifications for the rule
        var notifications = _db.Notifications.AsQueryable()
            .Where(n => n.RuleId == ruleId)
            .ToList();

        return new NotificationRule
        {
            Id = rule.Id,
            Name = rule.Name,
            SensorId = rule.SensorId,
            Measurement = rule.Measurement,
            Operator = rule.Operator,
            Value = rule.Value,
            Notifications = notifications
        };
    }

    /// <summary>
    /// Create a new notification rule.
    /// </summary>
    /// <returns><see cref="NotificationRule"/> or null if the rule couldn't be created</returns>
    public NotificationRule? CreateNotificationRule(NotificationRule rule)
    {
        rule.Id = null;
        // InsertOne doesnt return a result, so we need to wrap it in a try-catch block
        try
        {
            _db.NotificationRules.InsertOneAsync(rule);
        }
        catch
        {
            return null;
        }

        return rule;
    }

    /// <summary>
    /// Delete a notification rule by its id.
    /// </summary>
    public bool DeleteNotificationRule(string ruleId)
    {
        var result = _db.NotificationRules.DeleteOne(r => r.Id == ruleId);

        return result.IsAcknowledged;
    }

    /// <summary>
    /// Update a notification rule.
    /// </summary>
    /// <returns>Success if rule was found and updated</returns>
    public ServiceResult UpdateNotificationRule(NotificationRule rule)
    {
        var result = _db.NotificationRules.ReplaceOne(r => r.Id == rule.Id, rule);

        if (result.ModifiedCount == 0)
            return ServiceResult.NotFound;

        return ServiceResult.Success;
    }
}
