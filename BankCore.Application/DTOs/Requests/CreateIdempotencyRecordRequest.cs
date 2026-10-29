namespace BankCore.Application.DTOs.Requests;

public record CreateIdempotencyRecordRequest(
    string Key,
    string RequestHash,
    Guid TransactionId,
    DateTime ExpiresAt);
