using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public DateTime DateOfBirth { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    private Customer()
    {
    }

    public Customer(
        int userId,
        string firstName,
        string lastName,
        DateTime dateOfBirth)
    {

        if (userId <= 0)
            throw new DomainException("User ID must be positive.");
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        if (dateOfBirth.Date >= DateTime.UtcNow.Date)
            throw new DomainException("Date of birth must be in the past.");

        UserId = userId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth.Date;
    }

    public void UpdateDetails(
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        MarkAsUpdated();
    }
}
