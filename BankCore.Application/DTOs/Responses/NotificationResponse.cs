namespace BankCore.Application.DTOs.Responses;

public record NotificationResponse(
    int Id,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt);
