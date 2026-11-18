using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BankCore.Infrastructure.Security;

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> passwordHasher;
    private readonly User dummyUser;
    private readonly string dummyPasswordHash;

    public PasswordService(IPasswordHasher<User> passwordHasher)
    {
        this.passwordHasher = passwordHasher;

        dummyUser = new User("timing-check@bankcore.invalid");

        dummyPasswordHash = passwordHasher.HashPassword(dummyUser,"BankCore-Dummy-Password-9281!");
    }

    public string HashPassword(User user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User? user, string password)
    {
        if (user is null)
        {
            passwordHasher.VerifyHashedPassword(
                dummyUser,
                dummyPasswordHash,
                password);

            return false;
        }

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
