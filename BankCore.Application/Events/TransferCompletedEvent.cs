namespace BankCore.Application.Events;

public record TransferCompletedEvent(
    Guid TransactionId,
    int FromCustomerId,
    int ToCustomerId,
    decimal Amount,
    string Currency,
    DateTime OccurredAt);