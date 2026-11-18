using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Helpers;

public class TransactionAmountValidator
{
    public const decimal MaximumAmount = 1_000_000m;

    public static decimal Validate(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Transaction amount must be positive.");

        if (amount > MaximumAmount)
            throw new DomainException("Transaction amount exceeds the maximum allowed.");

        if (decimal.Round(amount, 2) != amount)
            throw new DomainException("Transaction amount cannot have more than two decimal places.");

        return amount;
    }
}
