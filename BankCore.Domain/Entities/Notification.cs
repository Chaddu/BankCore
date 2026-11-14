using BankCore.Domain.Exceptions;
using System.Transactions;

namespace BankCore.Domain.Entities;

public class Notification : BaseEntity
{
    public int CustomerId { get; private set; }
    public Guid TransactionId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }

    public Customer Customer { get; private set; } = null!;

    private Notification() { }

    public Notification(int customerId, Guid transactionId, string title, string message)
    {
        if (customerId <= 0)
            throw new DomainException("Customer ID must be greater than zero.");
        if (transactionId == Guid.Empty)
            throw new DomainException("Transaction ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Notification title is required.");

        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Notification message is required.");

        CustomerId = customerId;
        TransactionId = transactionId;
        Title = title.Trim();
        Message = message.Trim();
        IsRead = false;
    }

    public void MarkAsRead()
    {
        if(IsRead)
            return;
        IsRead = true;
        MarkAsUpdated();
    }
}
