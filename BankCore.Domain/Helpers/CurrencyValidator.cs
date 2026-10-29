using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Helpers;
public class CurrencyValidator
{
    public static string Validate(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        currency = currency.Trim().ToUpperInvariant();

        if (currency.Length != 3 || !currency.All(char.IsAsciiLetter))
            throw new DomainException("Currency must contain exactly three letters.");

        return currency;
    }
}
