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
    public List<Notification> GetNotifications()
    {
        return _db.Notifications
            .Find(FilterDefinition<Notification>.Empty)
            .SortByDescending(n => n.StartTimestamp)
            .ToList();
    }

    // Notification Rules

    public List<NotificationRule> GetNotificationRules()
    {
        return _db.NotificationRules
            .Find(FilterDefinition<NotificationRule>.Empty)
            .ToList();
    }

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

    public bool DeleteNotificationRule(string ruleId)
    {
        var result = _db.NotificationRules.DeleteOne(r => r.Id == ruleId);

        return result.IsAcknowledged;
    }

    public ServiceResult UpdateNotificationRule(NotificationRule rule)
    {
        var result = _db.NotificationRules.ReplaceOne(r => r.Id == rule.Id, rule);

        if (result.ModifiedCount == 0)
            return ServiceResult.NotFound;

        return ServiceResult.Success;
    }
}
