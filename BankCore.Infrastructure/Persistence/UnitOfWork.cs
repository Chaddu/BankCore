using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Repositories;
using BankCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankCore.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public ICustomerRepository Customers { get; } =
        new CustomerRepository(context);

    public IBankAccountRepository BankAccounts { get; } =
        new BankAccountRepository(context);

    public ILedgerEntryRepository LedgerEntries { get; } =
        new LedgerEntryRepository(context);

    public ITransferRepository Transfers { get; } =
        new TransferRepository(context);

    public IIdempotencyRepository IdempotencyRecords { get; } =
        new IdempotencyRepository(context);

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("No transaction is in progress.");

        try
        {
            await _transaction.CommitAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is null)
            return;

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
