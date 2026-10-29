using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);

    Task<Customer?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(Customer customer);
}
