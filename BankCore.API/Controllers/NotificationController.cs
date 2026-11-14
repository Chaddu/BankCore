using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[Authorize]
public class NotificationController(INotificationService notificationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await notificationService.GetMyNotificationsAsync(GetUserId());

        return HandleResult(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await notificationService.GetUnreadCountAsync(GetUserId());

        return HandleResult(result);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await notificationService.MarkAsReadAsync(id, GetUserId());

        return HandleResult(result);
    }
}
