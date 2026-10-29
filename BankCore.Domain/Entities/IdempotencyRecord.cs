using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class IdempotencyRecord : BaseEntity
{
    public string Key { get; private set; } = string.Empty;

    public string RequestHash { get; private set; } = string.Empty;

    public Guid TransactionId { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    private IdempotencyRecord()
    {
    }

    public IdempotencyRecord(
        string key,
        string requestHash,
        Guid transactionId,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Idempotency key is required.");

        if (string.IsNullOrWhiteSpace(requestHash))
            throw new DomainException("Request hash is required.");

        if (transactionId == Guid.Empty)
            throw new DomainException("Transaction ID is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Expiration time must be in the future.");

        Key = key.Trim();
        RequestHash = requestHash;
        TransactionId = transactionId;
        ExpiresAt = expiresAt;
    }
}