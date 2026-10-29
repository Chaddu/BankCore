namespace BankCore.Application.DTOs.Responses;

public record IdempotencyRecordResponse(
    int Id,
    string Key,
    string RequestHash,
    Guid TransactionId,
    DateTime ExpiresAt,
    DateTime CreatedAt);