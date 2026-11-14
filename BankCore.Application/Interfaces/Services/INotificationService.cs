using BankCore.Application.Common;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;

public interface INotificationService
{
    Task<Result<List<NotificationResponse>>> GetMyNotificationsAsync(int currentUserId);

    Task<Result<int>> GetUnreadCountAsync(int currentUserId);

    Task<Result> MarkAsReadAsync(int notificationId, int currentUserId);
}
