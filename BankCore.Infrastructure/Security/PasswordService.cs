using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BankCore.Infrastructure.Security;

public class PasswordService(IPasswordHasher<User> passwordHasher) : IPasswordService
    {
    public string HashPassword(User user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result != PasswordVerificationResult.Failed;
    }
}
