using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;
public interface ILedgerEntryRepository
{
    Task<LedgerEntry?> GetByIdAsync(int id);

    Task<List<LedgerEntry>> GetByAccountIdAsync(int accountId,int page,int pageSize);

    Task<int> CountByAccountIdAsync(int accountId);

    Task AddAsync(LedgerEntry entry);
}
