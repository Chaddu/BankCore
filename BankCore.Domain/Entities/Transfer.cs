using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;
using BankCore.Domain.Helpers;

namespace BankCore.Domain.Entities;

public class Transfer : BaseEntity
{
    public int FromAccountId { get; private set; }

    public int ToAccountId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid TransactionId { get; private set; }
    public TransferStatus Status { get; private set; }

    private Transfer()
    {
    }

    public Transfer(
        int fromAccountId,
        int toAccountId,
        decimal amount,
        string currency,
        string? description = null)
    {
        if (fromAccountId <= 0)
            throw new DomainException("Source account ID must be positive.");

        if (toAccountId <= 0)
            throw new DomainException("Destination account ID must be positive.");

        if (fromAccountId == toAccountId)
            throw new DomainException("Source and destination accounts must be different.");

        if (amount <= 0)
            throw new DomainException("Transfer amount must be positive.");
            
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        Amount = amount;
        Currency = CurrencyValidator.Validate(currency);
        Description = description?.Trim();
        TransactionId = Guid.NewGuid();
        Status = TransferStatus.Pending;
    }
    public void Complete()
    {
        if (Status != TransferStatus.Pending)
            throw new DomainException("Only pending transfers can be completed.");

        Status = TransferStatus.Completed;
        MarkAsUpdated();
    }

}
