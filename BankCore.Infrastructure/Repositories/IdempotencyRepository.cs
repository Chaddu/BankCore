using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class IdempotencyRepository(AppDbContext context) : IIdempotencyRepository
{
    public async Task<IdempotencyRecord?> GetByKeyAsync(string key)
    {
        return await context.IdempotencyRecords.FirstOrDefaultAsync(i => i.Key == key);
    }

    public async Task AddAsync(IdempotencyRecord record)
    {
        await context.IdempotencyRecords.AddAsync(record);
    }
}
