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

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
