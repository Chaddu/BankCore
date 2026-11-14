using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);

    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByUserIdAsync(int userId);

    Task AddAsync(Customer customer);
}
