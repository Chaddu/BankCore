using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class LedgerEntryRepository(AppDbContext context) : ILedgerEntryRepository
{
    public async Task AddAsync(LedgerEntry entry)
    {
        await context.LedgerEntries.AddAsync(entry);
    }

    public async Task<int> CountByAccountIdAsync(int accountId)
    {
        return await context.LedgerEntries.CountAsync(l => l.BankAccountId == accountId);
    }

    public async Task<List<LedgerEntry>> GetByAccountIdAsync(int accountId, int page, int pageSize)
    {
        return await context.LedgerEntries
            .Where(l => l.BankAccountId == accountId)
            .OrderByDescending(l => l.CreatedAt)
            .ThenByDescending(l => l.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<LedgerEntry?> GetByIdAsync(int id)
    {
        return await context.LedgerEntries.FirstOrDefaultAsync(l => l.Id == id);
    }
}
