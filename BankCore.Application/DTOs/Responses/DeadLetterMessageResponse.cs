namespace BankCore.Application.DTOs.Responses;

public record DeadLetterMessageResponse(
    string MessageId,
    string ContentType,
    string Body,
    DateTime? Timestamp,
    string? Reason);