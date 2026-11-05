using System.Security.Cryptography;

namespace BankCore.Application.Common;

public class AccountNumberGenerator
{
    public static string Generate()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000_000);

        return $"BC{number:D9}";
    }
}
