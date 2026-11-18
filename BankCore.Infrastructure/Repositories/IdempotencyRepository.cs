using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class IdempotencyRepository(AppDbContext context) : IIdempotencyRepository
{
    public async Task<IdempotencyRecord?> GetByKeyAsync(int customerId,string key)
    {
        return await context.IdempotencyRecords.FirstOrDefaultAsync(i => i.CustomerId == customerId && i.Key == key);
    }

    public async Task AddAsync(IdempotencyRecord record)
    {
        await context.IdempotencyRecords.AddAsync(record);
    }

    public Task DeleteAsync(IdempotencyRecord record)
    {
        context.IdempotencyRecords.Remove(record);
        return Task.CompletedTask;
    }
}
