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
            .ToList();
    }

    public bool AcknowledgeNotification(string notificationId)
    {
        var update = Builders<Notification>.Update.Set(n => n.IsAcknowledged, true);
        var result = _db.Notifications.UpdateOne(n => n.Id == notificationId, update);

        return true;
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
        var query = _db.NotificationRules.AsQueryable()
            .GroupJoin(_db.Notifications.AsQueryable(),
                rule => rule.Id,
                notification => notification.RuleId,
                (rule, notifications) => new NotificationRule
                {
                    Id = rule.Id,
                    Name = rule.Name,
                    SensorId = rule.SensorId,
                    Measurement = rule.Measurement,
                    Operator = rule.Operator,
                    Value = rule.Value,
                    Notifications = notifications.ToList()
                });

        return query.FirstOrDefault();
    }

    public bool CreateNotificationRule(NotificationRule rule)
    {
        // InsertOne doesnt return a result, so we need to wrap it in a try-catch block
        try
        {
            _db.NotificationRules.InsertOne(rule);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool DeleteNotificationRule(string ruleId)
    {
        var result = _db.NotificationRules.DeleteOne(r => r.Id == ruleId);

        return result.IsAcknowledged;
    }

    public ServiceResult UpdateNotificationRule(NotificationRule rule)
    {
        var result = _db.NotificationRules.ReplaceOne(r => r.Id == rule.Id, rule);

        if (!result.IsAcknowledged)
            return ServiceResult.BadRequest;

        if (result.ModifiedCount == 0)
            return ServiceResult.NotFound;

        return ServiceResult.Success;
    }
}
