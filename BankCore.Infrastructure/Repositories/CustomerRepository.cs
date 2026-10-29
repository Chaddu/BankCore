using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task AddAsync(Customer customer)
    {
        await context.Customers.AddAsync(customer);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();

        return await context.Customers.AnyAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();

        return await context.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }
}
