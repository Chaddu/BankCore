using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;
using BankCore.Domain.Helpers;

namespace BankCore.Domain.Entities;
public class BankAccount : BaseEntity
{
    public string AccountNumber { get; private set; } = string.Empty;

    public int OwnerId { get; private set; }

    public decimal Balance { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public AccountStatus Status { get; private set; }


    private BankAccount()
    {
        
    }

    public BankAccount(int ownerId, string accountNumber, string currency)
    {
        if (ownerId <= 0)
            throw new DomainException("Owner ID must be positive.");

        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new DomainException("Account number is required.");

        OwnerId = ownerId;
        AccountNumber = accountNumber.Trim();
        Currency = CurrencyValidator.Validate(currency);
        Balance = 0m;
        Status = AccountStatus.Active;
    }

    public void Deposit(decimal amount)
    {
        EnsureActive();
        if(amount <=0)
            throw new DomainException("Deposit amount must be positive.");

        Balance += amount;
        MarkAsUpdated();
    }
    public void Withdraw(decimal amount)
    {
        EnsureActive();

        if(amount <= 0)
            throw new DomainException("Withdrawal amount must be positive.");

        if(amount > Balance)
            throw new DomainException("Insufficient funds.");

        Balance -= amount;
        MarkAsUpdated();
    }

    public void Freeze()
    {
        if (Status != AccountStatus.Active)
            throw new DomainException("Only active accounts can be frozen.");

        Status = AccountStatus.Frozen;
        MarkAsUpdated();
    }

    public void Close()
    {
        if (Status == AccountStatus.Closed)
            throw new DomainException("Account is already closed.");

        if (Balance != 0m)
            throw new DomainException("Account balance must be zero before closing.");

        Status = AccountStatus.Closed;
        MarkAsUpdated();
    }

    public void Reactivate()
    {
        if(Status != AccountStatus.Frozen)
            throw new DomainException("Only frozen accounts can be reactivated.");

        Status = AccountStatus.Active;
        MarkAsUpdated();
    }

    private void EnsureActive()
    {
        if(Status != AccountStatus.Active)
            throw new DomainException("Account is not active.");
    }
}
