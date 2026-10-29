using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class LedgerEntry : BaseEntity
{
    public int BankAccountId { get; private set; }

    public decimal Amount { get; private set; }

    public TransactionType Type { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Guid TransactionId { get; private set; }

    private LedgerEntry()
    {
    }

    public LedgerEntry(int bankAccountId,
        decimal amount,
        TransactionType type,
        string description,
        Guid transactionId)
    {
        if(bankAccountId <= 0)
            throw new DomainException("Bank account ID must be positive.");

        if (amount <= 0)
            throw new DomainException("Amount must be positive.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required.");

        if (transactionId == Guid.Empty)
            throw new DomainException("Transaction ID is required.");

        if (!Enum.IsDefined(typeof(TransactionType), type))
            throw new DomainException("Invalid transaction type.");

        BankAccountId = bankAccountId;
        Amount = amount;
        Type = type;
        Description = description.Trim();
        TransactionId = transactionId;

    }


}
