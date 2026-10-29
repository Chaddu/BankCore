using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(string key);

    Task AddAsync(IdempotencyRecord record);
}
