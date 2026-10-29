using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;
public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(int id);

    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);

    Task<bool> ExistsByAccountNumberAsync(string accountNumber);

    Task<List<BankAccount>> GetByOwnerIdAsync(int ownerId);

    Task AddAsync(BankAccount account);
}
