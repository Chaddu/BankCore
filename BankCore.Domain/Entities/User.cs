using BankCore.Domain.Enums;
using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private User() { }

    public User(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        Email = email.Trim().ToLowerInvariant();
        Role = UserRole.Customer;
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        PasswordHash = passwordHash;
        MarkAsUpdated();
    }

    public void ChangeRole(UserRole role)
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new DomainException("Invalid user role.");

        Role = role;
        MarkAsUpdated();
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        PasswordHash = passwordHash;
    }
}