using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(int customerId, string key);

    Task AddAsync(IdempotencyRecord record);

    Task DeleteAsync(IdempotencyRecord record);
}
