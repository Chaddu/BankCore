using BankCore.Application.Interfaces.Repositories;

namespace BankCore.Application.Interfaces;
public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }

    IBankAccountRepository BankAccounts { get; }

    ILedgerEntryRepository LedgerEntries { get; }

    ITransferRepository Transfers { get; }

    IIdempotencyRepository IdempotencyRecords { get; }
    IUserRepository Users { get; }
    INotificationRepository Notifications { get; }
    IOutboxRepository OutboxMessages { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
