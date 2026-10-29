using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

internal class BankAccountRepository(AppDbContext context) : IBankAccountRepository
{
    public async Task<BankAccount?> GetByIdAsync(int id)
    {
        return await context.BankAccounts.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await context.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<bool> ExistsByAccountNumberAsync(string accountNumber)
    {
        return await context.BankAccounts.AnyAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<List<BankAccount>> GetByOwnerIdAsync(int ownerId)
    {
        return await context.BankAccounts.Where(a => a.OwnerId == ownerId).ToListAsync();
    }

    public async Task AddAsync(BankAccount account)
    {
        await context.BankAccounts.AddAsync(account);
    }
}
