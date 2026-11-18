using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class IdempotencyRecord : BaseEntity
{
    public int CustomerId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public string RequestHash { get; private set; } = string.Empty;

    public Guid TransactionId { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    private IdempotencyRecord()
    {
    }

    public IdempotencyRecord(
        int customerId,
        string key,
        string requestHash,
        Guid transactionId,
        DateTime expiresAt)
    {
        if (customerId <= 0)
            throw new DomainException("Customer ID must be positive.");

        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Idempotency key is required.");

        key = key.Trim();

        if (key.Length > 128)
            throw new DomainException("Idempotency key cannot exceed 128 characters.");

        if (string.IsNullOrWhiteSpace(requestHash))
            throw new DomainException("Request hash is required.");

        if (transactionId == Guid.Empty)
            throw new DomainException("Transaction ID is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Expiration time must be in the future.");

        CustomerId = customerId;
        Key = key;
        RequestHash = requestHash;
        TransactionId = transactionId;
        ExpiresAt = expiresAt;
    }
}