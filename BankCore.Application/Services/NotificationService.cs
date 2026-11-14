using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;

namespace BankCore.Application.Services;

public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : INotificationService
{
    public async Task<Result<List<NotificationResponse>>> GetMyNotificationsAsync(int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<List<NotificationResponse>>.Failure("Customer not found", ErrorType.NotFound);

        var notifications = await unitOfWork.Notifications.GetByCustomerIdAsync(customer.Id);

        var response = mapper.Map<List<NotificationResponse>>(notifications);

        return Result<List< NotificationResponse >>.Success(response);
    }

    public async Task<Result<int>> GetUnreadCountAsync(int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if(customer is null)
            return Result<int>.Failure("Customer not found", ErrorType.NotFound);

        var count = await unitOfWork.Notifications.CountUnreadAsync(customer.Id);

        return Result<int>.Success(count);
    }

    public async Task<Result> MarkAsReadAsync(int notificationId, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result.Failure("Customer not found", ErrorType.NotFound);

        var notification = await unitOfWork.Notifications.GetByIdAsync(notificationId);

        if (notification is null || notification.CustomerId != customer.Id)
            return Result.Failure("Notification not found.", ErrorType.NotFound);

        notification.MarkAsRead();

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
