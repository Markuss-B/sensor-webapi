using Microsoft.AspNetCore.Mvc;
using SensorWebApi.Models;
using SensorWebApi.Services;

namespace SensorWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public NotificationsController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    // GET: api/Notifications
    [HttpGet]
    public ActionResult<List<Notification>> GetNotifications()
    {
        return _notificationsService.GetNotifications();
    }

    // GET: api/Notifications/rules
    [HttpGet("rules")]
    public ActionResult<List<NotificationRule>> GetNotificationRules()
    {
        return _notificationsService.GetNotificationRules();
    }

    // GET: api/Notifications/rules/5
    [HttpGet("rules/{ruleId}")]
    public ActionResult<NotificationRule> GetNotificationRule(string ruleId)
    {
        var rule = _notificationsService.GetNotificationRule(ruleId);

        if (rule == null)
            return NotFound();

        return Ok(rule);
    }

    // POST: api/Notifications/rules/new
    [HttpPost("rules/new")]
    public ActionResult<NotificationRule> CreateNotificationRule(NotificationRule rule)
    {
        var result = _notificationsService.CreateNotificationRule(rule);

        if (result == null)
            return BadRequest();

        return Ok(rule);
    }

    // DELETE: api/Notifications/rules/5
    [HttpDelete("rules/{ruleId}")]
    public ActionResult DeleteNotificationRule(string ruleId)
    {
        _notificationsService.DeleteNotificationRule(ruleId);

        return Ok();
    }

    [HttpPut("rules")]
    public ActionResult UpdateNotificationRule(NotificationRule rule)
    {
        var result = _notificationsService.UpdateNotificationRule(rule);

        if (result.IsNotFound)
            return NotFound();

        if (result.IsBadRequest)
            return BadRequest();

        return Ok();
    }
}
